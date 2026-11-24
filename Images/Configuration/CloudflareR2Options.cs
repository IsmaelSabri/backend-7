namespace Images.Configuration;

public sealed class CloudflareR2Options
{
    public const string SectionName = "CloudflareR2";
    public required string AccountId { get; init; }
    public required string S3AccessId { get; init; }
    public required string S3SecretKey { get; init; }
    public required string BucketName { get; init; }
    public required string PublicUrl { get; init; }
}
