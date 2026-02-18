using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace Homes.Models
{
    [Table("Flat")]
    public class Flat : House
    {
        public bool Jardin { get; set; }
        public bool Columpios { get; set; }
        public bool Tenis { get; set; }
        public bool Padel { get; set; }
        public bool Sauna { get; set; }
        public bool BajoOplantabaja { get; set; }
        public bool VideoPortero { get; set; }
        [Sieve(CanFilter = true)]
        public bool ZonaDeOcio { get; set; }
    }
}