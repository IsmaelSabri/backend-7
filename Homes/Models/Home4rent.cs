using System.ComponentModel.DataAnnotations.Schema;

namespace Homes.Models
{
    [Table("Home4rent")]
    public class Home4rent : Flat
    {
        public string? Mascotas { get; set; }
        public int Fianza { get; set; }
        public string? Disponibilidad { get; set; }
        public int EstanciaMinima { get; set; }
    }
}