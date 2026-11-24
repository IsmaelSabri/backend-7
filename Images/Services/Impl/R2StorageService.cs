using Amazon.S3;
using Amazon.S3.Model;
using Images.Configuration;
using Microsoft.Extensions.Options;

namespace Images.Services.Impl;

public sealed class R2StorageService(
    IAmazonS3 client,
    IOptions<CloudflareR2Options> options,
    ILogger<R2StorageService> logger) : IImageStorageService
{
    private readonly CloudflareR2Options _options = options.Value;

    public async Task<string> UploadAsync(Stream stream, string ownerType, string ownerId, CancellationToken ct)
    {
        var key = ImageStorageLayout.CreateKey(ownerType, ownerId);
        try
        {
            await client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = ImageStorageLayout.RawKey(key),
                InputStream = stream,
                ContentType = "application/octet-stream",
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true,
                AutoCloseStream = false
            }, ct);
        }
        catch
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            try { await DeleteAsync(key, timeout.Token); }
            catch (Exception ex) { logger.LogError(ex, "No se pudo limpiar el original R2 de {StorageKey}", key); }
            throw;
        }
        return key;
    }

    public async Task DeleteAsync(string key, CancellationToken ct)
    {
        foreach (var (suffix, _) in ImageStorageLayout.GetVariants(key))
            await client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = ImageStorageLayout.VariantKey(key, suffix)
            }, ct);
        await client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = ImageStorageLayout.RawKey(key)
        }, ct);
    }

    public ImageUrls GetUrls(string key)
    {
        var baseUrl = _options.PublicUrl.TrimEnd('/');
        string Url(string suffix) => $"{baseUrl}/{string.Join("/", ImageStorageLayout.VariantKey(key, suffix).Split('/').Select(Uri.EscapeDataString))}";
        if (ImageStorageLayout.IsNewProfileKey(key))
            return new ImageUrls(null, null, null, Url(".avatar.webp"), Url(".webp"));
        if (ImageStorageLayout.IsNewEnergyCertificateKey(key))
            return new ImageUrls(null, null, Url(".webp"), null, null);
        var thumbnail = Url(".thumbnail.webp");
        var card = Url(".card.webp");
        var full = Url(".webp");
        return ImageStorageLayout.IsProfileKey(key)
            ? new ImageUrls(thumbnail, card, full, thumbnail, full)
            : new ImageUrls(thumbnail, card, full, null, null);
    }
}
