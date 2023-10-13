using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Homes.Models
{
    [Table("HolidayRent")]
    public class HolidayRent : Home
    {
        public string? NumeroRegistro { get; set; }
        public string? Personas { get; set; }
        public string? Camas { get; set; }
        public bool Toallas { get; set; }
        public bool Sabanas { get; set; }
        public bool Tv { get; set; }
        public bool TvCable { get; set; }
        public bool Microondas { get; set; }
        public bool Lavavajillas { get; set; }
        public bool Lavadora { get; set; }
        public bool Secadora { get; set; }
        public bool Cafetera { get; set; }
        public bool Plancha { get; set; }
        public bool Cuna { get; set; }
        public bool SecadorDePelo { get; set; }
        public bool Wifi { get; set; }
        public bool PrimeraLineaPlaya { get; set; }
        public bool Chimenea { get; set; }
        public bool MueblesJardin { get; set; }
        public bool Barbacoa { get; set; }
        public string? ValoracionesUsuarios { get; set; }
        public int Valoraciones { get; set; }
        public int StarRatingAverage { get; set; }

    }
}