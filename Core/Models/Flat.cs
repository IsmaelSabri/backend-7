using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace Core.Models
{
    [Table("Flat")]
    public class Flat : House
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Jardin { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Columpios { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Tenis { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Padel { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Sauna { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool BajoOplantabaja { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool VideoPortero { get; set; }
        [Sieve(CanFilter = true)]
        public bool ZonaDeOcio { get; set; }
    }
}