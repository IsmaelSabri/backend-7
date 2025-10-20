using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sieve.Attributes;

namespace Users.Models
{
    public class Image
    {
        [Key]
        public string? Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        [Sieve(CanFilter = true)]
        public string? ImageId { get; set; }
        public string? ImageDeleteUrl { get; set; }
    }
}