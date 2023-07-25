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
        public string? Ciudad { get; set; }
        public int? Cp { get; set; }
        public string? Habitaciones { get; set; }
        public string? Aseos { get; set; }
        public string? Superficie { get; set; }
        public string? Condicion { get; set; }
        public string? Tipo { get; set; }
        public bool? Ascensor { get; set; }
        public string? PrecioInicial { get; set; }
        public string? Descuento { get; set; }
        public string? PrecioFinal { get; set; }
        public string? Duracion { get; set; }
        public string? Descripcion { get; set; }
        public bool? AireAcondicionado { get; set; }
        public bool? Calefaccion { get; set; }
        public string? PanelesSolares { get; set; }
        public string? ArmariosEmpotrados { get; set; }
        public string? Terraza { get; set; }
        public bool? Parquet { get; set; }
        public string? Balcon { get; set; }
        public string? Antiguedad { get; set; }
        public string? Garage { get; set; }
        public string? MetroMasProximo { get; set; }
        public string? AutobusMasProximo { get; set; }
        public string? Estado { get; set; }
        public string? Jardin { get; set; }
        public string? Piscina { get; set; }
        public bool? Trastero { get; set; }
        public string? VistasDespejadas { get; set; }
        public string? DistanciaAlMar { get; set; }
        public string? BajoOplantabaja { get; set; }
        public bool? InstalacionesDiscapacitados { get; set; }
        public string? NuevoProyecto { get; set; }
        public string? Creador { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public string? NumeroVisitas { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        public string? ImageId { get; set; }
        public string? Video { get; set; }
        // url
        // alquiler
        public bool? Mascotas { get; set; }
        public int? Fianza { get; set; }
        public string? Disponibilidad { get; set; }
        public int? EstanciaMinima { get; set; } // meses
        // compartir
        public bool? SepuedeFumar { get; set; }
        public bool? SeadmitenParejas { get; set; }
        public bool? SeadmitenMenoresdeedad { get; set; }
        public string? PerfilCompartir { get; set; }
        public int? HabitantesActualmente { get; set; }
        public bool? PropietarioviveEnlacasa { get; set; }
        public string? Ambiente { get; set; }
        public bool? Internet { get; set; }
        public string? Gastos { get; set; }
        public int? Numero { get; set; }
        public string? Comentario { get; set; }
        public string? Calle { get; set; }
    }
}