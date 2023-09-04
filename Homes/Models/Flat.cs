using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Homes.Models
{
    [Table("Flat")]
    public class Flat : House
    {
        public bool? Ascensor { get; set; }
        public bool? Trastero { get; set; }
        public bool? InstalacionesDiscapacitados { get; set; }
        public bool? Jardin { get; set; }
        public bool? PiscinaComp { get; set; }
        public bool? Columpios { get; set; }
        public bool? Gym { get; set; }
        public bool? Tenis { get; set; }
        public bool? Padel { get; set; }
        public bool? Sauna { get; set; }
        public bool? Jacuzzi { get; set; }
        public bool? Golf { get; set; }
        public string? VistasDespejadas { get; set; }
        public string? BajoOplantabaja { get; set; }
        public string? Puerta { get; set; }
        public string? Piso { get; set; }
        public string? Balcon { get; set; }

    }
}