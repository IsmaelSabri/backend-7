using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sieve.Attributes;

namespace Users.Models
{
    public class User
    {
        [Key]
        [Sieve(CanFilter = true)]
        public string? Id { get; set; }
        [Sieve(CanFilter = true)]
        public string? UserId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        [Sieve(CanFilter = true)]
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImageAsString { get; set; }
        public string? BrandImageAsString { get; set; }
        public string? Color { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Password { get; set; }
        [Sieve(CanFilter = true)]
        public string? Email { get; set; }
        [Sieve(CanFilter = true)]
        public string? Role { get; set; }
        public bool Isactive { get; set; }
        public bool IsnotLocked { get; set; }
        [Sieve(CanFilter = true)]
        public string? Token { get; set; }
        [Sieve(CanFilter = true)]
        public string? RefreshToken { get; set; }
        public DateTime LastaccessDate { get; set; }
        public DateTime DateRegistry { get; set; }
        public DateTime RefreshTokenDateExpires { get; set; }
        [Sieve(CanFilter = true)]
        public string? Company { get; set; }
        public string? ReviewsAsString { get; set; }
        [Sieve(CanFilter = true)]
        public bool IsPro { get; set; }
        public string? Status { get; set; }
        public string[]? ChatsOpened { get; set; }
        public string[]? BlockedUsers { get; set; }
        public ICollection<Order>? Orders { get; } = new List<Order>(); // Collection navigation containing dependents
    }
}