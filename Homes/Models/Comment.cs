using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homes.Models
{
    public class Comment
    {
        public string? Id { get; set; }
        public string? Body { get; set; }
        public string? Author { get; set; }
        public string? AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}