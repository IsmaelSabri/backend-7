using Amazon.S3;
using Amazon.S3.Model;
using Images.Configuration;
using Images.Services;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace ImageWorker;

public sealed class ImageProcessor(IAmazonS3 client, IOptions<CloudflareR2Options> options)
{
    private readonly CloudflareR2Options _options = options.Value;

    public async Task ProcessAsync(string key, CancellationToken ct)
    {
        using var response = await client.GetObjectAsync(_options.BucketName, ImageStorageLayout.RawKey(key), ct);
        if (response.ContentLength <= 0 || response.ContentLength > 32L * 1024 * 1024)
            throw new ArgumentException("El archivo debe tener entre 1 byte y 32 MB.");
        using var stream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(stream, ct);
        if (stream.Length != response.ContentLength)
            throw new InvalidOperationException("El original R2 esta incompleto.");
        stream.Position = 0;
        using var codec = SKCodec.Create(stream) ?? throw new ArgumentException("Formato de imagen no compatible.");
        if (codec.FrameCount > 1)
            throw new ArgumentException("Las imagenes animadas no son compatibles.");
        if (codec.Info.Width <= 0 || codec.Info.Height <= 0 || (long)codec.Info.Width * codec.Info.Height > 40_000_000)
            throw new ArgumentException("Las dimensiones de la imagen no son validas o son demasiado grandes.");
        using var decoded = SKBitmap.Decode(codec) ?? throw new ArgumentException("No se pudo leer la imagen.");
        using var image = Orient(decoded, codec.EncodedOrigin);
        foreach (var (suffix, width) in ImageStorageLayout.GetVariants(key))
        {
            ct.ThrowIfCancellationRequested();
            var targetWidth = Math.Min(width, image.Width);
            var targetHeight = Math.Max(1, (int)Math.Round((double)image.Height * targetWidth / image.Width));
            using var resized = image.Resize(new SKImageInfo(targetWidth, targetHeight), new SKSamplingOptions(SKFilterMode.Linear))
                ?? throw new InvalidOperationException("No se pudo redimensionar la imagen.");
            using var skImage = SKImage.FromBitmap(resized);
            using var encoded = skImage.Encode(SKEncodedImageFormat.Webp, 82)
                ?? throw new InvalidOperationException("No se pudo codificar la imagen.");
            using var data = new MemoryStream(encoded.ToArray());
            await client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = ImageStorageLayout.VariantKey(key, suffix),
                InputStream = data,
                ContentType = "image/webp",
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true,
                AutoCloseStream = false
            }, ct);
        }
    }

    private static SKBitmap Orient(SKBitmap source, SKEncodedOrigin origin)
    {
        var w = source.Width;
        var h = source.Height;
        var rotated = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
            or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var result = new SKBitmap(rotated ? h : w, rotated ? w : h);
        using var canvas = new SKCanvas(result);
        var matrix = origin switch
        {
            SKEncodedOrigin.TopRight => new SKMatrix(-1, 0, w, 0, 1, 0, 0, 0, 1),
            SKEncodedOrigin.BottomRight => new SKMatrix(-1, 0, w, 0, -1, h, 0, 0, 1),
            SKEncodedOrigin.BottomLeft => new SKMatrix(1, 0, 0, 0, -1, h, 0, 0, 1),
            SKEncodedOrigin.LeftTop => new SKMatrix(0, 1, 0, 1, 0, 0, 0, 0, 1),
            SKEncodedOrigin.RightTop => new SKMatrix(0, -1, h, 1, 0, 0, 0, 0, 1),
            SKEncodedOrigin.RightBottom => new SKMatrix(0, -1, h, -1, 0, w, 0, 0, 1),
            SKEncodedOrigin.LeftBottom => new SKMatrix(0, 1, 0, -1, 0, w, 0, 0, 1),
            _ => SKMatrix.Identity
        };
        canvas.SetMatrix(matrix);
        canvas.DrawBitmap(source, 0, 0, new SKSamplingOptions(SKFilterMode.Linear));
        return result;
    }
}
