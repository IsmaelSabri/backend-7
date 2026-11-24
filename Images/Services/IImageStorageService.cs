namespace Images.Services;

public sealed record ImageUrls(
    string? ThumbnailUrl,
    string? CardUrl,
    string? FullUrl,
    string? AvatarUrl,
    string? CropperUrl);

public interface IImageStorageService
{
    Task<string> UploadAsync(Stream stream, string ownerType, string ownerId, CancellationToken ct);
    Task DeleteAsync(string key, CancellationToken ct);
    ImageUrls GetUrls(string key);
}
