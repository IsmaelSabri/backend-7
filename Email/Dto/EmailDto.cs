using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Email.Dto
{
    public class EmailDto
    {
        public string? Name { get; set; }
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}