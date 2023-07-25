using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homes.Dto
{
    public class HomeDto
    {
        public string? ViviendaId { get; set; }
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public string? Ciudad { get; set; }
        public string? Cp { get; set; }
        public string? Habitaciones { get; set; }
        public string? Aseos { get; set; }
        public string? Superficie { get; set; }
        public string? Condicion { get; set; }
        public string? Tipo { get; set; }
        public string? Ascensor { get; set; }
        public string? PrecioInicial { get; set; }
        public string? Descuento { get; set; }
        public string? PrecioFinal { get; set; }
        public string? Duracion { get; set; }
        public string? Descripcion { get; set; }
        public string? AireAcondicionado { get; set; }
        public string? Calefaccion { get; set; }
        public string? PanelesSolares { get; set; }
        public string? ArmariosEmpotrados { get; set; }
        public string? Terraza { get; set; }
        public string? Parquet { get; set; }
        public string? Balcon { get; set; }
        public string? Antiguedad { get; set; }
        public string? Garage { get; set; }
        public string? MetroMasProximo { get; set; }
        public string? AutobusMasProximo { get; set; }
        public string? Estado { get; set; }
        public string? Jardin { get; set; }
        public string? Piscina { get; set; }
        public string? Trastero { get; set; }
        public string? VistasDespejadas { get; set; }
        public string? DistanciaAlMar { get; set; }
        public string? BajoOplantabaja { get; set; }
        public string? InstalacionesDiscapacitados { get; set; }
        public string? NuevoProyecto { get; set; }
        public string? Creador { get; set; }
        public string? NumeroVisitas { get; set; }
        public string? Video { get; set; }
        public string? Mascotas { get; set; }
        public string? Fianza { get; set; }
        public string? Disponibilidad { get; set; }
        public string? EstanciaMinima { get; set; } // meses
        public string? SepuedeFumar { get; set; }
        public string? SeadmitenParejas { get; set; }
        public string? SeadmitenMenoresdeedad { get; set; }
        public string? PerfilCompartir { get; set; }
        public string? HabitantesActualmente { get; set; }
        public string? PropietarioviveEnlacasa { get; set; }
        public string? Ambiente { get; set; }
        public string? Internet { get; set; }
        public string? Gastos { get; set; }
        public string? Numero { get; set; }
        public string? Comentario { get; set; }
        public string? Calle { get; set; }
        public IFormFile? Foto { get; set; }
    }
}