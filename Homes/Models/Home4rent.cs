using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Homes.Models
{
    [Table("Home4rent")]
    public class Home4rent : Flat
    {
        public bool? Mascotas { get; set; }
        public int? Fianza { get; set; }
        public string? Disponibilidad { get; set; }
        public int? EstanciaMinima { get; set; }
    }
}