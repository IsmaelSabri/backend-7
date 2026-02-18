using System;
using System.ComponentModel.DataAnnotations;
using Users.Models;

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
        // Use proper types for boolean flags
        public bool? Isactive { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        // Date fields as DateTime
        public DateTime? LastaccessDate { get; set; }
        public DateTime? DateRegistry { get; set; }
        public DateTime? RefreshTokenDateExpires { get; set; }
        public string? Company { get; set; }
        public bool? IsPro { get; set; }
        public string? Status { get; set; }
    }
}