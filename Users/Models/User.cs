using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sieve.Attributes;
using Images.Models;

namespace Users.Models
{
    public class User
    {
        [Key]
        [Sieve(CanFilter = true)]
        public Guid Id { get; set; }
        [Sieve(CanFilter = true)]
        public string? UserId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        [Sieve(CanFilter = true)]
        public string? Username { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Password { get; set; }
        public string? Phone { get; set; }
        [Sieve(CanFilter = true)]
        public string? Email { get; set; }
        public DateTime LastaccessDate { get; set; }
        public DateTime DateRegistry { get; set; }
        //public string? ProfileImageAsString { get; set; }
        [Sieve(CanFilter = true)]
        public string? Role { get; set; }
        [Sieve(CanFilter = true)]
        public Guid OrganizationId { get; private set; }

        public bool Isactive { get; set; }
        [Sieve(CanFilter = true)]
        public string? Token { get; set; }
        [Sieve(CanFilter = true)]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenDateExpires { get; set; }
        public string? Status { get; set; }
        [Sieve(CanFilter = true)]
        public string[]? BlockedUsers { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Image? ProfileImage { get; set; }
    }
}