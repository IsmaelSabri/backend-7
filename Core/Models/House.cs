using System.ComponentModel.DataAnnotations.Schema;
using Images.Models;
using Sieve.Attributes;

namespace Core.Models
{
    [Table("House")]
    public class House : Home
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Alarma { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Recepcion24_7 { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool VideoVigilancia { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool AlarmaIncendios { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Extintores { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Consumo { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Emisiones { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Aerotermia { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool VentilacionCruzada { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool DobleAcristalamiento { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool GeneradorEmergencia { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Gym { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Golf { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Alturas { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? TotalViviendasConstruidas { get; set; }
    }
}