using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Homes.Models
{
    [Table("House")]
    public class House : Home
    {
        public House(){}
        public bool Alarma { get; set; }
        public bool Recepcion24_7 { get; set; }
        public bool VideoVigilancia { get; set; }
        public bool AlarmaIncendios { get; set; }
        public bool Extintores { get; set; }
        public bool Calefaccion { get; set; }
        public bool PanelesSolares { get; set; }
        public bool EficienciaEnergetica { get; set; } // ver etiquetas
        public string? Colegios { get; set; }
        public string? Supermercados { get; set; }
        public string? Aeropuerto { get; set; }
        public string? Consumo { get; set; }
        public string? Emisiones { get; set; }
        public bool GeneradorEmergencia { get; set; }    
    }
}