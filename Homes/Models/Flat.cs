using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace Homes.Models
{
    [Table("Flat")]
    public class Flat : House
    {
        [Sieve(CanFilter = true)]
        public bool Trastero { get; set; }
        public bool Jardin { get; set; }
        [Sieve(CanFilter = true)]
        public bool PiscinaComp { get; set; }
        public bool Columpios { get; set; }
        public bool Gym { get; set; }
        public bool Tenis { get; set; }
        public bool Padel { get; set; }
        public bool Sauna { get; set; }
        public bool Golf { get; set; }
        public bool BajoOplantabaja { get; set; }
    }
}