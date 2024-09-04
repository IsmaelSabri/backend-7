using System.ComponentModel.DataAnnotations.Schema;

namespace Homes.Models
{
    [Table("NewProject")]
    public class NewProject : Flat
    {
        public int PorcentajeVendido { get; set; }
        public int PorcentajeTerminado { get; set; }
        public string? NombreProyecto { get; set; }
        public string? EstadoConstruccion { get; set; }
        public string? Tipos { get; set; }
        public int HabitacionesDesde { get; set; }
        public int HabitacionesHasta { get; set; }
        public int SuperficieDesde { get; set; }
        public int SuperficieHasta { get; set; }
        public int FinDeObra { get; set; }
        public int Alturas { get; set; }
        public int TotalViviendasConstruidas { get; set; }
    }
}