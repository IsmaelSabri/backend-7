using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Images.Collections.Impl;
using Images.Configuration;
using Images.Data;
using Images.Models;
using Images.Services;
using Images.Services.Impl;
using ImageWorker;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;
using RabbitMQ.Client;
using Sieve.Models;
using Sieve.Services;
using Xunit;

public sealed class ImageQueueIntegrationTests
{
    [ImageQueueIntegrationFact]
    public async Task PublishesMetadataProcessesAndSerializesDeletion()
    {
        var rootConnection = Environment.GetEnvironmentVariable("IMAGE_TEST_POSTGRES")!;
        var rabbitPort = Environment.GetEnvironmentVariable("IMAGE_TEST_RABBIT_PORT")!;
        var schema = "image_test_" + Guid.NewGuid().ToString("N");
        await using var admin = new NpgsqlConnection(rootConnection);
        await admin.OpenAsync();
        await using (var create = new NpgsqlCommand($"CREATE SCHEMA \"{schema}\"", admin))
            await create.ExecuteNonQueryAsync();
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(rootConnection) { SearchPath = schema };
            var settings = Options.Create(new CloudflareR2Options
            {
                AccountId = "account", S3AccessId = "access", S3SecretKey = "secret",
                BucketName = "images", PublicUrl = "https://images.example.test"
            });
            using var s3 = new BlockingS3();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RabbitMQ:Host"] = "localhost", ["RabbitMQ:Port"] = rabbitPort
            }).Build();
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IAmazonS3>(s3)
                .AddSingleton<IOptions<CloudflareR2Options>>(settings)
                .AddSingleton<ImageProcessor>()
                .AddDbContext<ImageDb>(options => options.UseNpgsql(builder.ConnectionString))
                .BuildServiceProvider();
            await using var provider = services;
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ImageDb>();
            await db.Database.ExecuteSqlRawAsync("""
                CREATE TABLE "Images" (
                    "Id" uuid PRIMARY KEY, "StorageKey" varchar(255) NOT NULL,
                    "AltText" varchar(150), "DisplayOrder" integer NOT NULL,
                    "IsCover" boolean NOT NULL, "OwnerId" varchar(64) NOT NULL,
                    "OwnerType" varchar(32) NOT NULL, "CreatedAt" timestamptz NOT NULL
                )
                """);
            var script = await File.ReadAllTextAsync(Path.GetFullPath(
                "../../../../Images/Sql/image-processing.sql", AppContext.BaseDirectory));
            await db.Database.ExecuteSqlRawAsync(script);
            var manager = new ImageManager(new ImageCollection(db), R2StorageTests.CreateStorage(s3),
                new SieveProcessor(Options.Create(new SieveOptions())), NullLogger<ImageManager>.Instance);
            var imageBytes = R2StorageTests.CreateImage(1000, 500);
            var file = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "files", "test.jpg");
            var uploaded = await manager.UploadAsync(file, "owner-1", "HomeImages", default);
            Assert.Equal(ImageStatuses.Processing, uploaded.Status);
            Assert.Null(uploaded.FullUrl);
            Assert.Single(s3.Uploads);

            var publisher = ActivatorUtilities.CreateInstance<ImageQueuePublisher>(provider);
            var worker = ActivatorUtilities.CreateInstance<ImageConsumer>(provider);
            try
            {
                await using var connection = await ImageQueue.CreateFactory(configuration).CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(ImageQueue.Name, durable: true, exclusive: false, autoDelete: false);
                configuration["RabbitMQ:Port"] = "1";
                await publisher.StartAsync(default);
                await Task.Delay(2500);
                using (var pendingScope = provider.CreateScope())
                {
                    var pendingDb = pendingScope.ServiceProvider.GetRequiredService<ImageDb>();
                    var pending = await pendingDb.Images.AsNoTracking().SingleAsync(item => item.Id == uploaded.Id);
                    Assert.Equal(ImageStatuses.Processing, pending.Status);
                    Assert.Null(pending.QueuedAt);
                }
                configuration["RabbitMQ:Port"] = rabbitPort;
                BasicGetResult? delivery = null;
                for (var attempt = 0; attempt < 100 && delivery == null; attempt++)
                {
                    delivery = await channel.BasicGetAsync(ImageQueue.Name, autoAck: false);
                    if (delivery == null) await Task.Delay(100);
                }
                Assert.NotNull(delivery);
                using var json = JsonDocument.Parse(delivery.Body);
                Assert.Equal(5, json.RootElement.EnumerateObject().Count());
                Assert.Equal(uploaded.Id, json.RootElement.GetProperty("ImageId").GetGuid());
                Assert.StartsWith("raw/homeimages/owner-1/", json.RootElement.GetProperty("RawKey").GetString());
                await channel.BasicNackAsync(delivery.DeliveryTag, false, true);

                s3.BlockVariants = true;
                await worker.StartAsync(default);
                await s3.VariantStarted.Task.WaitAsync(TimeSpan.FromSeconds(15));
                var deletion = manager.DeleteAsync(uploaded.Id, default);
                await Task.Delay(200);
                Assert.False(deletion.IsCompleted);
                s3.AllowVariant.TrySetResult();
                Assert.True(await deletion.WaitAsync(TimeSpan.FromSeconds(15)));
                await Task.Delay(200);
                Assert.Empty(s3.Uploads);

                s3.BlockVariants = false;
                var readyFile = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "files", "ready.jpg");
                var ready = await manager.UploadAsync(readyFile, "owner-2", "ProfileImage", default);
                s3.FailOnPut = 2;
                var readyImage = await WaitForStatus(provider, ready.Id, ImageStatuses.Ready);
                s3.FailOnPut = 0;
                var rawKey = ImageStorageLayout.RawKey(readyImage.StorageKey);
                await WaitUntilAsync(() => !s3.Uploads.ContainsKey(rawKey));
                Assert.Equal(2, s3.Uploads.Count);
                int deletedBefore;
                lock (s3.DeletedKeys) deletedBefore = s3.DeletedKeys.Count(key => key == rawKey);
                var duplicate = new ProcessImageCommand(ready.Id, rawKey,
                    ImageStorageLayout.VariantKey(readyImage.StorageKey, ""), readyImage.OwnerType, readyImage.OwnerId);
                await channel.BasicPublishAsync("", ImageQueue.Name, JsonSerializer.SerializeToUtf8Bytes(duplicate));
                await WaitUntilAsync(() =>
                {
                    lock (s3.DeletedKeys) return s3.DeletedKeys.Count(key => key == rawKey) > deletedBefore;
                });
                Assert.Equal(2, s3.Uploads.Count);
                Assert.True(await manager.DeleteAsync(ready.Id, default));
                Assert.Empty(s3.Uploads);

                byte[] invalidBytes = [1, 2, 3];
                var invalidFile = new FormFile(new MemoryStream(invalidBytes), 0, invalidBytes.Length, "files", "bad.jpg");
                var invalid = await manager.UploadAsync(invalidFile, "owner-3", "EnergyCertImage", default);
                var failed = await WaitForStatus(provider, invalid.Id, ImageStatuses.Failed);
                Assert.True(s3.Uploads.ContainsKey(ImageStorageLayout.RawKey(failed.StorageKey)));

                var replacementFile = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "files", "cert.jpg");
                var replacement = await manager.UploadAsync(replacementFile, "owner-3", "EnergyCertImage", default);
                await WaitForStatus(provider, replacement.Id, ImageStatuses.Ready);
                var results = await manager.QueryAsync(new SieveModel { Filters = "ownerId==owner-3" }, default);
                Assert.Equal(replacement.Id, results[0].Id);
                Assert.Equal(ImageStatuses.Ready, results[0].Status);
                Assert.True(await manager.DeleteAsync(invalid.Id, default));
                Assert.True(await manager.DeleteAsync(replacement.Id, default));
                Assert.Empty(s3.Uploads);
            }
            finally
            {
                s3.AllowVariant.TrySetResult();
                await worker.StopAsync(default);
                await publisher.StopAsync(default);
                worker.Dispose();
                publisher.Dispose();
            }
        }
        finally
        {
            await using var drop = new NpgsqlCommand($"DROP SCHEMA \"{schema}\" CASCADE", admin);
            await drop.ExecuteNonQueryAsync();
        }
    }

    private static async Task<Image> WaitForStatus(ServiceProvider provider, Guid id, string status)
    {
        for (var attempt = 0; attempt < 150; attempt++)
        {
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ImageDb>();
            var image = await db.Images.AsNoTracking().SingleAsync(item => item.Id == id);
            if (image.Status == status) return image;
            await Task.Delay(100);
        }
        throw new TimeoutException($"La imagen {id} no alcanzo el estado {status}.");
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 150; attempt++)
        {
            if (condition()) return;
            await Task.Delay(100);
        }
        throw new TimeoutException("No se completo la operacion de R2.");
    }

    private sealed class BlockingS3 : R2StorageTests.FakeS3
    {
        public bool BlockVariants { get; set; }
        public TaskCompletionSource VariantStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource AllowVariant { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override async Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken ct = default)
        {
            if (BlockVariants && !request.Key.StartsWith("raw/", StringComparison.Ordinal))
            {
                VariantStarted.TrySetResult();
                await AllowVariant.Task.WaitAsync(ct);
            }
            return await base.PutObjectAsync(request, ct);
        }
    }
}

public sealed class ImageQueueIntegrationFactAttribute : FactAttribute
{
    public ImageQueueIntegrationFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IMAGE_TEST_POSTGRES"))
            || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IMAGE_TEST_RABBIT_PORT")))
            Skip = "Requires isolated PostgreSQL and RabbitMQ test instances.";
    }
}
