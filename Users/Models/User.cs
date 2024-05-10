using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Users.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public string? Domains { get; set; }
        public string? ProfileImageAsString { get; set; }
        public string? BrandImageAsString { get; set; }
        public string? Color { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool Isactive { get; set; }
        public bool IsnotLocked { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime LastaccessDate { get; set; }
        public DateTime DateRegistry { get; set; }
        public DateTime RefreshTokenDateExpires { get; set; }
        public string? LikePreferencesAsString { get; set; }
        public string? Company { get; set; }
        public string? ReviewsAsString { get; set; }
        public bool IsPro { get; set; }
    }
}