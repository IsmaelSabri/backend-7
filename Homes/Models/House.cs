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
        public bool? Alarma { get; set; }
        public bool? Recepcion24_7 { get; set; }
        public bool? VideoVigilancia { get; set; }
        public bool? AlarmaIncendios { get; set; }
        public bool? Extintores { get; set; }
        public bool? AireAcondicionado { get; set; }
        public bool? Calefaccion { get; set; }
        public bool? PanelesSolares { get; set; }
        public bool? AltaEficienciaEnergetica { get; set; }
        public string? NombreColegios { get; set; }
        public string? EnseñanzaColegios { get; set; }
        public string? InstitucionColegios { get; set; }
        public string? DistanciaColegios { get; set; }
        public string? LineaMetro { get; set; }
        public string? ParadaMetro { get; set; }
        public string? DistanciaMetro { get; set; }
        public string? LineaBus { get; set; }
        public string? ParadaBus { get; set; }
        public string? DistanciaBus { get; set; }
    }
}