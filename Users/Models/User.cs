using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Users.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string? UserId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
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
    }
}