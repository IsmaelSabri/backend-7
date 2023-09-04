using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Homes.Models
{
    public class Home
    {
        [Key]
        public int? Id { get; set; }
        public string? ViviendaId { get; set; }
        public Double? Lat { get; set; }
        public Double? Lng { get; set; }
        public string? Calle { get; set; }
        public int? Numero { get; set; }
        public string? Ciudad { get; set; }
        public int? Cp { get; set; }
        public string? Habitaciones { get; set; }
        public string? Aseos { get; set; }
        public bool? AseoEnsuite { get; set; }
        public string? Superficie { get; set; }
        public string? Condicion { get; set; }
        public string? Tipo { get; set; }
        public string? PrecioInicial { get; set; }
        public string? Descuento { get; set; }
        public string? PrecioFinal { get; set; }
        public string? PrecioM2 { get; set; }
        public string? Duracion { get; set; }
        public string? Descripcion { get; set; }
        public string? ArmariosEmpotrados { get; set; }
        public string? Terraza { get; set; }
        public bool? PiscinaPrivada { get; set; }
        public bool? Parquet { get; set; }
        public string? Antiguedad { get; set; }
        public string? Garage { get; set; }
        public string? Estado { get; set; }
        public string? DistanciaAlMar { get; set; }
        public string? Creador { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public string? NumeroVisitas { get; set; }
        public string? Comentario { get; set; }
        public string? Destacar { get; set; }
        public string? Model { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        public string? ImageId { get; set; }
        public string? Video { get; set; }
    }
}