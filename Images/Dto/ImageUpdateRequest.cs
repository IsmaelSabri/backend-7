using System.ComponentModel.DataAnnotations;

namespace Images.Dto;

/// <summary>Updates image display metadata.</summary>
public sealed record ImageUpdateRequest
{
    [MaxLength(150)]
    public string? AltText { get; init; }
    [Range(0, int.MaxValue)]
    public int Order { get; init; }
    public bool IsCover { get; init; }
}
