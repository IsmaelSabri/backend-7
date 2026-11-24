namespace Images.Dto;

/// <summary>Image URLs and ordering information for display.</summary>
public sealed record ImageResponse(
    Guid Id,
    string? ThumbnailUrl,
    string? CardUrl,
    string? FullUrl,
    int Order,
    bool IsCover,
    string OwnerId,
    string OwnerType,
    string? AltText,
    string? AvatarUrl,
    string? CropperUrl,
    string Status);
