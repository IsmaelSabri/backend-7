using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.Dto
{
    public class UserReadyDto
    {
        public string? Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserId { get; set; }
        public string? Role { get; set; }
        public DateTime DateRegistry { get; set; }

    }
}