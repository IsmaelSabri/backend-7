using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace Homes.Models
{
    [Table("House")]
    public class House : Home
    {
        public bool Alarma { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Recepcion24_7 { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool VideoVigilancia { get; set; }
        public bool AlarmaIncendios { get; set; }
        public bool Extintores { get; set; }
        public bool EficienciaEnergetica { get; set; } // ver etiquetas
        public string? Colegios { get; set; }
        public string? Supermercados { get; set; }
        public string? Aeropuerto { get; set; }
        public string? Consumo { get; set; }
        public string? Emisiones { get; set; }
        public bool Aerotermia { get; set; }
        public bool VentilacionCruzada { get; set; }
        public bool DobleAcristalamiento { get; set; }
        public string? EnergyCertAsString { get; set; }
        public bool GeneradorEmergencia { get; set; }
        public bool Gym { get; set; }
        public bool Golf { get; set; }
        public string? Universidades { get; set; }
        public string? Metro { get; set; }
        public string? Bus { get; set; }
        public int Alturas { get; set; }
        public int TotalViviendasConstruidas { get; set; }
    }
}