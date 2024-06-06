using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;

namespace Homes.Models
{
    //[Table("House")]
    public class House : Home
    {
        public bool Alarma { get; set; }
        public bool Recepcion24_7 { get; set; }
        public bool VideoVigilancia { get; set; }
        public bool AlarmaIncendios { get; set; }
        public bool Extintores { get; set; }
        [Sieve(CanFilter = true)]
        public bool PanelesSolares { get; set; }
        public bool EficienciaEnergetica { get; set; } // ver etiquetas
        public string? Colegios { get; set; }
        public string? Supermercados { get; set; }
        public string? Aeropuerto { get; set; }
        public string? Consumo { get; set; }
        public string? Emisiones { get; set; }
        public bool GeneradorEmergencia { get; set; }
        [Sieve(CanFilter = true)]
        public string? VistasDespejadas { get; set; }
        [Sieve(CanFilter = true)]
        public bool InstalacionesDiscapacitados { get; set; }
        [Sieve(CanFilter = true)]
        public bool Jacuzzi { get; set; }
    }
}