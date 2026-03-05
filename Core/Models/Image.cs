using System.ComponentModel.DataAnnotations;
using Sieve.Attributes;

namespace Images.Models
{
    public sealed class Image
    {
        [Key]
        [Required]
        public string? Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        [Sieve(CanFilter = true)]
        public string? OwnerId { get; set; }
        [Sieve(CanFilter = true)]
        public string? OwnerType { get; set; }
        public string? ImageDeleteUrl { get; set; }
    }
}