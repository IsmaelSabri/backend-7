using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace Core.Models
{
    [Table("NewProject")]
    public class NewProject : Flat
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PorcentajeVendido { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PorcentajeTerminado { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? NombreProyecto { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? EstadoConstruccion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string[]? Tipos { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? HabitacionesDesde { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? HabitacionesHasta { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? SuperficieDesde { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? SuperficieHasta { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? FinDeObra { get; set; }
    }
}