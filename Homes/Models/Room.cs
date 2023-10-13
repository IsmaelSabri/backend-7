using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Homes.Models
{
    [Table("Room")]
    public class Room : Home
    {
        public bool SepuedeFumar { get; set; }
        public bool SeadmitenParejas { get; set; }
        public bool SeadmitenMenoresdeedad { get; set; }
        public bool SeadmitenMochileros { get; set; }
        public bool SeadmitenJubilados { get; set; }
        public bool SeadmiteLGTBI { get; set; }
        public bool PropietarioviveEnlacasa { get; set; }
        public string? PerfilCompartir { get; set; }
        public string? HabitantesActualmente { get; set; }
        public string? Ambiente { get; set; }
        public string? Gastos { get; set; }
    }
}