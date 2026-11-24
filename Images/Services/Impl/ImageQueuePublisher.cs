using System.Text.Json;
using Images.Data;
using Images.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace Images.Services.Impl;

public sealed class ImageQueuePublisher(
    IServiceScopeFactory scopes,
    IConfiguration configuration,
    ILogger<ImageQueuePublisher> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishPendingAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex) { logger.LogError(ex, "No se pudieron publicar las imagenes pendientes"); }
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private async Task PublishPendingAsync(CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ImageDb>();
        var pending = await db.Images.AsNoTracking()
            .Where(image => image.Status == ImageStatuses.Processing && image.QueuedAt == null)
            .OrderBy(image => image.CreatedAt).Take(100).ToListAsync(ct);
        if (pending.Count == 0) return;

        await using var connection = await ImageQueue.CreateFactory(configuration).CreateConnectionAsync(ct);
        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true), ct);
        await channel.QueueDeclareAsync(ImageQueue.Name, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
        foreach (var image in pending)
        {
            var command = new ProcessImageCommand(image.Id, ImageStorageLayout.RawKey(image.StorageKey),
                ImageStorageLayout.VariantKey(image.StorageKey, ""), image.OwnerType, image.OwnerId);
            var body = JsonSerializer.SerializeToUtf8Bytes(command);
            await channel.BasicPublishAsync("", ImageQueue.Name, mandatory: true,
                new BasicProperties { Persistent = true, ContentType = "application/json" }, body, ct);
            await db.Images.Where(item => item.Id == image.Id && item.Status == ImageStatuses.Processing && item.QueuedAt == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.QueuedAt, DateTimeOffset.UtcNow), ct);
        }
    }
}
