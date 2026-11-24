using System.ComponentModel.DataAnnotations;
using Sieve.Attributes;

namespace Images.Models
{
    public sealed class Image
    {
        [Key]
        [Required]
        public string? Id { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? CardUrl { get; set; }
        public string? FullUrl { get; set; }
        public string? AvatarUrl { get; set; }
        public string? CropperUrl { get; set; }
        public string? Status { get; set; }
        public string? AltText { get; set; }
        public int Order { get; set; }
        public bool IsCover { get; set; }
        [Sieve(CanFilter = true)]
        public string? OwnerId { get; set; }
        [Sieve(CanFilter = true)]
        public string? OwnerType { get; set; }
    }
}