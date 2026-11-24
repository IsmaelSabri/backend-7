using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Concurrent;
using Images.Configuration;
using Images.Services;
using Images.Services.Impl;
using ImageWorker;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SkiaSharp;
using Xunit;

public sealed class R2StorageTests
{
    [Theory]
    [InlineData(2000, 1000, 500, 800, 1600)]
    [InlineData(250, 125, 250, 250, 250)]
    public async Task UploadCreatesAndDeletesSizedVariants(int width, int height, int thumbnail, int card, int full)
    {
        using var s3 = new FakeS3();
        var storage = CreateStorage(s3);
        var key = await storage.UploadAsync(new MemoryStream(CreateImage(width, height)), "HomeImages", "owner-1", default);
        await ProcessAndRemoveRaw(s3, key);
        Assert.StartsWith("homeimages/owner-1/", key);
        var urls = storage.GetUrls(key);
        Assert.Equal($"https://images.example.test/{key}", urls.FullUrl);
        Assert.Equal(3, s3.Uploads.Count);
        foreach (var (url, expected) in new[] { (urls.ThumbnailUrl, thumbnail), (urls.CardUrl, card), (urls.FullUrl, full) })
        {
            Assert.NotNull(url);
            var data = s3.Uploads[new Uri(url).AbsolutePath.TrimStart('/')];
            using var image = SKBitmap.Decode(data);
            Assert.Equal(expected, image.Width);
            Assert.Equal(expected / 2, image.Height);
        }
        await storage.DeleteAsync(key, default);
        Assert.Empty(s3.Uploads);
    }

    [Theory]
    [InlineData(1200, 600, 192, 560)]
    [InlineData(100, 50, 100, 100)]
    public async Task ProfileUploadCreatesOnlyAvatarAndCropperVariants(int width, int height, int avatar, int cropper)
    {
        using var s3 = new FakeS3();
        var storage = CreateStorage(s3);
        var key = await storage.UploadAsync(new MemoryStream(CreateImage(width, height)), "ProfileImage", "owner-1", default);
        await ProcessAndRemoveRaw(s3, key);
        Assert.StartsWith("profileimage/owner-1/p2-", key);
        var urls = storage.GetUrls(key);
        Assert.Null(urls.ThumbnailUrl);
        Assert.Null(urls.CardUrl);
        Assert.Null(urls.FullUrl);
        Assert.Equal($"https://images.example.test/{key}", urls.CropperUrl);
        Assert.Equal(2, s3.Uploads.Count);
        foreach (var (url, expected) in new[] { (urls.AvatarUrl, avatar), (urls.CropperUrl, cropper) })
        {
            Assert.NotNull(url);
            using var image = SKBitmap.Decode(s3.Uploads[new Uri(url).AbsolutePath.TrimStart('/')]);
            Assert.Equal(expected, image.Width);
            Assert.Equal(expected / 2, image.Height);
        }
        await storage.DeleteAsync(key, default);
        Assert.Empty(s3.Uploads);
        Assert.Equal(3, s3.DeletedKeys.Count);
    }

    [Theory]
    [InlineData(1200, 600, 560)]
    [InlineData(250, 125, 250)]
    public async Task EnergyCertificateUploadCreatesOneSizedVariant(int width, int height, int expectedWidth)
    {
        using var s3 = new FakeS3();
        var storage = CreateStorage(s3);
        var key = await storage.UploadAsync(new MemoryStream(CreateImage(width, height)), "EnergyCertImage", "owner-1", default);
        await ProcessAndRemoveRaw(s3, key);
        Assert.StartsWith("energycertimage/owner-1/e2-", key);
        var urls = storage.GetUrls(key);
        Assert.Equal($"https://images.example.test/{key}", urls.FullUrl);
        Assert.Null(urls.ThumbnailUrl);
        Assert.Null(urls.CardUrl);
        Assert.Null(urls.AvatarUrl);
        Assert.Null(urls.CropperUrl);
        Assert.Single(s3.Uploads);
        using var image = SKBitmap.Decode(s3.Uploads[key]);
        Assert.Equal(expectedWidth, image.Width);
        Assert.Equal(expectedWidth / 2, image.Height);
        await storage.DeleteAsync(key, default);
        Assert.Empty(s3.Uploads);
        Assert.Equal(new[] { key, ImageStorageLayout.RawKey(key) }, s3.DeletedKeys);
    }

    [Fact]
    public async Task PreviousEnergyCertificateRetainsItsUrlsAndDeletesAllVariants()
    {
        using var s3 = new FakeS3();
        var storage = CreateStorage(s3);
        const string key = "energycertimage/owner-1/old.webp";
        var urls = storage.GetUrls(key);
        Assert.Equal("https://images.example.test/energycertimage/owner-1/old.thumbnail.webp", urls.ThumbnailUrl);
        Assert.Equal("https://images.example.test/energycertimage/owner-1/old.card.webp", urls.CardUrl);
        Assert.Equal($"https://images.example.test/{key}", urls.FullUrl);
        await storage.DeleteAsync(key, default);
        Assert.Equal(new[] {
            "energycertimage/owner-1/old.thumbnail.webp",
            "energycertimage/owner-1/old.card.webp",
            key,
            ImageStorageLayout.RawKey(key)
        }, s3.DeletedKeys);
    }

    [Fact]
    public async Task FailedEnergyCertificateUploadCleansUpItsObject()
    {
        using var s3 = new FakeS3 { FailOnPut = 2 };
        var storage = CreateStorage(s3);
        var key = await storage.UploadAsync(new MemoryStream(CreateImage(100, 50)), "EnergyCertImage", "owner-1", default);
        await Assert.ThrowsAsync<AmazonS3Exception>(() =>
            CreateProcessor(s3).ProcessAsync(key, default));
        Assert.Single(s3.Uploads);
        await storage.DeleteAsync(key, default);
        Assert.Empty(s3.Uploads);
        Assert.Equal(2, s3.DeletedKeys.Count);
    }

    [Fact]
    public async Task PreviousProfileVariantsRemainAccessibleAndDeletable()
    {
        using var s3 = new FakeS3();
        var storage = CreateStorage(s3);
        const string key = "profileimage/owner-1/old.webp";
        var urls = storage.GetUrls(key);
        Assert.Equal(urls.ThumbnailUrl, urls.AvatarUrl);
        Assert.Equal(urls.FullUrl, urls.CropperUrl);
        await storage.DeleteAsync(key, default);
        Assert.Equal(new[] {
            "profileimage/owner-1/old.thumbnail.webp",
            "profileimage/owner-1/old.card.webp",
            key,
            ImageStorageLayout.RawKey(key)
        }, s3.DeletedKeys);
    }

    [Fact]
    public async Task FailedProfileUploadCleansUpBothVariants()
    {
        using var s3 = new FakeS3 { FailOnPut = 3 };
        var storage = CreateStorage(s3);
        var key = await storage.UploadAsync(new MemoryStream(CreateImage(100, 50)), "ProfileImage", "owner-1", default);
        await Assert.ThrowsAsync<AmazonS3Exception>(() =>
            CreateProcessor(s3).ProcessAsync(key, default));
        Assert.Equal(2, s3.Uploads.Count);
        await storage.DeleteAsync(key, default);
        Assert.Empty(s3.Uploads);
        Assert.Equal(3, s3.DeletedKeys.Count);
    }

    [Fact]
    public async Task FailedUploadCleansUpPartialObjects()
    {
        using var s3 = new FakeS3 { FailOnPut = 1 };
        var storage = CreateStorage(s3);
        await Assert.ThrowsAsync<AmazonS3Exception>(() =>
            storage.UploadAsync(new MemoryStream(CreateImage(100, 50)), "HomeImages", "owner-1", default));
        Assert.Empty(s3.Uploads);
    }

    [Fact]
    public async Task InvalidImageDoesNotUploadAnything()
    {
        using var s3 = new FakeS3();
        var storage = CreateStorage(s3);
        var key = await storage.UploadAsync(new MemoryStream("invalid"u8.ToArray()), "HomeImages", "owner-1", default);
        await Assert.ThrowsAsync<ArgumentException>(() => CreateProcessor(s3).ProcessAsync(key, default));
        Assert.Single(s3.Uploads);
    }

    [Fact]
    public async Task ExifOrientationIsAppliedBeforeResizing()
    {
        using var s3 = new FakeS3();
        var storage = CreateStorage(s3);
        var jpeg = CreateImage(400, 200);
        byte[] exif =
        [
            0xff, 0xe1, 0x00, 0x22, 0x45, 0x78, 0x69, 0x66, 0x00, 0x00,
            0x49, 0x49, 0x2a, 0x00, 0x08, 0x00, 0x00, 0x00, 0x01, 0x00,
            0x12, 0x01, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        ];
        var orientedJpeg = jpeg[..2].Concat(exif).Concat(jpeg[2..]).ToArray();
        var key = await storage.UploadAsync(new MemoryStream(orientedJpeg), "HomeImages", "owner-1", default);
        await ProcessAndRemoveRaw(s3, key);
        using var result = SKBitmap.Decode(s3.Uploads[key]);
        Assert.Equal(200, result.Width);
        Assert.Equal(400, result.Height);
    }

    internal static R2StorageService CreateStorage(FakeS3 s3) => new(s3,
        Options.Create(new CloudflareR2Options
        {
            AccountId = "account", S3AccessId = "access", S3SecretKey = "secret",
            BucketName = "images", PublicUrl = "https://images.example.test"
        }), NullLogger<R2StorageService>.Instance);

    private static ImageProcessor CreateProcessor(FakeS3 s3) => new(s3,
        Options.Create(new CloudflareR2Options
        {
            AccountId = "account", S3AccessId = "access", S3SecretKey = "secret",
            BucketName = "images", PublicUrl = "https://images.example.test"
        }));

    private static async Task ProcessAndRemoveRaw(FakeS3 s3, string key)
    {
        await CreateProcessor(s3).ProcessAsync(key, default);
        s3.Uploads.TryRemove(ImageStorageLayout.RawKey(key), out _);
    }

    internal static byte[] CreateImage(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Red);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
        return data.ToArray();
    }

    internal class FakeS3 : AmazonS3Client
    {
        public ConcurrentDictionary<string, byte[]> Uploads { get; } = new();
        public List<string> DeletedKeys { get; } = [];
        public int FailOnPut { get; set; }
        public bool FailOnDelete { get; set; }
        private int _puts;

        public FakeS3() : base(new BasicAWSCredentials("fake", "fake"), RegionEndpoint.USEast1) { }

        public override async Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken ct = default)
        {
            if (FailOnPut > 0 && ++_puts == FailOnPut) throw new AmazonS3Exception("simulated failure");
            using var buffer = new MemoryStream();
            await request.InputStream.CopyToAsync(buffer, ct);
            Uploads[request.Key] = buffer.ToArray();
            return new PutObjectResponse();
        }

        public override Task<GetObjectResponse> GetObjectAsync(string bucketName, string key, CancellationToken ct = default)
        {
            var data = Uploads[key];
            return Task.FromResult(new GetObjectResponse { ResponseStream = new MemoryStream(data), ContentLength = data.Length });
        }

        public override Task<DeleteObjectResponse> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken ct = default)
        {
            if (FailOnDelete) throw new AmazonS3Exception("simulated failure");
            lock (DeletedKeys) DeletedKeys.Add(request.Key);
            Uploads.TryRemove(request.Key, out _);
            return Task.FromResult(new DeleteObjectResponse());
        }
    }
}
