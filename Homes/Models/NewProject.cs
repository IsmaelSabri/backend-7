using System.ComponentModel.DataAnnotations.Schema;

namespace Homes.Models
{
    [Table("NewProject")]
    public class NewProject : House
    {
        public string? Planificacion { get; set; }
        public string? InicioDeVentas { get; set; }
        public string? InicioConstruccion { get; set; }
        public string? Mudandose { get; set; }
    }
}