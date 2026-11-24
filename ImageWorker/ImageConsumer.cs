using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Images.Configuration;
using Images.Data;
using Images.Models;
using Images.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ImageWorker;

public sealed class ImageConsumer(
    IServiceScopeFactory scopes,
    ImageProcessor processor,
    IAmazonS3 s3,
    IOptions<CloudflareR2Options> options,
    IConfiguration configuration,
    ILogger<ImageConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = await ImageQueue.CreateFactory(configuration).CreateConnectionAsync(stoppingToken);
                var disconnected = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                connection.ConnectionShutdownAsync += (_, _) =>
                {
                    disconnected.TrySetResult();
                    return Task.CompletedTask;
                };
                await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
                await channel.QueueDeclareAsync(ImageQueue.Name, durable: true, exclusive: false, autoDelete: false,
                    cancellationToken: stoppingToken);
                await channel.BasicQosAsync(0, 2, false, stoppingToken);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, delivery) =>
                {
                    try
                    {
                        var command = JsonSerializer.Deserialize<ProcessImageCommand>(delivery.Body.Span)
                            ?? throw new JsonException("Mensaje vacio.");
                        await ConsumeAsync(command, stoppingToken);
                        await channel.BasicAckAsync(delivery.DeliveryTag, false, stoppingToken);
                    }
                    catch (JsonException ex)
                    {
                        logger.LogError(ex, "Mensaje de procesamiento de imagen no valido");
                        await channel.BasicRejectAsync(delivery.DeliveryTag, false, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "No se pudo confirmar el procesamiento; se reintentara");
                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                        await channel.BasicNackAsync(delivery.DeliveryTag, false, true, stoppingToken);
                    }
                };
                await channel.BasicConsumeAsync(ImageQueue.Name, autoAck: false, consumer, stoppingToken);
                await disconnected.Task.WaitAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                logger.LogError(ex, "No se pudo conectar al broker de imagenes");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ConsumeAsync(ProcessImageCommand command, CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ImageDb>();
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var image = await db.Images.FromSqlInterpolated(
            $"SELECT * FROM \"Images\" WHERE \"Id\" = {command.ImageId} FOR UPDATE").SingleOrDefaultAsync(ct);
        if (image == null)
        {
            await transaction.CommitAsync(ct);
            return;
        }
        if (command.RawKey != ImageStorageLayout.RawKey(image.StorageKey)
            || command.TargetBaseKey != ImageStorageLayout.VariantKey(image.StorageKey, "")
            || command.OwnerId != image.OwnerId || command.OwnerType != image.OwnerType)
            throw new JsonException("El mensaje no coincide con la imagen guardada.");

        if (image.Status == ImageStatuses.Processing)
        {
            Exception? failure = null;
            for (var attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    await processor.ProcessAsync(image.StorageKey, ct);
                    failure = null;
                    break;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
                catch (ArgumentException ex) { failure = ex; break; }
                catch (Exception ex)
                {
                    failure = ex;
                    logger.LogWarning(ex, "Fallo procesando imagen {ImageId}, intento {Attempt}", image.Id, attempt);
                    if (attempt < 3) await Task.Delay(TimeSpan.FromSeconds(2 * attempt), ct);
                }
            }
            image.Status = failure == null ? ImageStatuses.Ready : ImageStatuses.Failed;
            if (failure != null)
                logger.LogError(failure, "No se pudo procesar la imagen {ImageId}", image.Id);
            await db.SaveChangesAsync(ct);
        }
        var ready = image.Status == ImageStatuses.Ready;
        await transaction.CommitAsync(ct);
        if (ready)
            await s3.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = options.Value.BucketName,
                Key = command.RawKey
            }, ct);
    }
}
