using System.ComponentModel.DataAnnotations;
using Sieve.Attributes;

namespace Images.Models;

public sealed class Image
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public required string StorageKey { get; set; }

    [MaxLength(150)]
    public string? AltText { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsCover { get; set; }

    [Required, MaxLength(64), Sieve(CanFilter = true)]
    public required string OwnerId { get; set; }

    [Required, MaxLength(32), Sieve(CanFilter = true)]
    public required string OwnerType { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    [Required, MaxLength(16)]
    public string Status { get; set; } = ImageStatuses.Ready;
    public DateTimeOffset? QueuedAt { get; set; }
}

public static class ImageStatuses
{
    public const string Processing = "Processing";
    public const string Ready = "Ready";
    public const string Failed = "Failed";
}
