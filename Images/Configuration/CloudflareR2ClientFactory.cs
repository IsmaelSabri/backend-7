using Amazon.Runtime;
using Amazon.S3;

namespace Images.Configuration;

public static class CloudflareR2ClientFactory
{
    public static IAmazonS3 Create(CloudflareR2Options settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(settings.AccountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(settings.S3AccessId);
        ArgumentException.ThrowIfNullOrWhiteSpace(settings.S3SecretKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(settings.BucketName);
        if (!Uri.TryCreate(settings.PublicUrl, UriKind.Absolute, out var publicUrl)
            || publicUrl.Scheme != Uri.UriSchemeHttps || !string.IsNullOrEmpty(publicUrl.UserInfo)
            || !string.IsNullOrEmpty(publicUrl.Query) || !string.IsNullOrEmpty(publicUrl.Fragment))
            throw new InvalidOperationException("CloudflareR2:PublicUrl debe ser una URL HTTPS valida.");
        if (!settings.AccountId.All(char.IsAsciiLetterOrDigit))
            throw new InvalidOperationException("CloudflareR2:AccountId no es valido.");
        return new AmazonS3Client(new BasicAWSCredentials(settings.S3AccessId, settings.S3SecretKey),
            new AmazonS3Config
            {
                ServiceURL = $"https://{settings.AccountId}.r2.cloudflarestorage.com",
                AuthenticationRegion = "auto",
                ForcePathStyle = true
            });
    }
}
