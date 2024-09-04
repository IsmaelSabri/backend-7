
using System.ComponentModel.DataAnnotations;

namespace Users.Dto
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImageAsString { get; set; }
        public string? BrandImageAsString { get; set; }
        public string? Color { get; set; }
        public string? Password { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Isactive { get; set; }
        public string? IsnotLocked { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? LastaccessDate { get; set; }
        public string? DateRegistry { get; set; }
        public string? RefreshTokenDateExpires { get; set; }
        public string? LikePreferencesAsString { get; set; }
        public string? Company { get; set; }
        public string? ReviewsAsString { get; set; }
        public string? IsPro { get; set; }
        public string? Status { get; set; }
        public string? ChatsOpenedAsString { get; set; }
    }
}