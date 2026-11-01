using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace Core.Models
{
    [Table("Room")]
    public class Room : House
    {
        [Sieve(CanFilter = true)]
        public bool SepuedeFumar { get; set; }
        [Sieve(CanFilter = true)]
        public bool SeadmitenParejas { get; set; }
        [Sieve(CanFilter = true)]
        public bool SeadmitenMenoresdeedad { get; set; }
        [Sieve(CanFilter = true)]
        public bool SeadmitenMochileros { get; set; }
        [Sieve(CanFilter = true)]
        public bool SeadmitenJubilados { get; set; }
        [Sieve(CanFilter = true)]
        public bool SeadmiteLGTBI { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool PropietarioviveEnlacasa { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? PerfilCompartir { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? HabitantesActualmente { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Ambiente { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Gastos { get; set; }
    }
}