using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace Homes.Models
{
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
        public bool PropietarioviveEnlacasa { get; set; }
        public string? PerfilCompartir { get; set; }
        public string? HabitantesActualmente { get; set; }
        public string? Ambiente { get; set; }
        public string? Gastos { get; set; }
    }
}