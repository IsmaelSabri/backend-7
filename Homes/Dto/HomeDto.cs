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
        public string? Calle { get; set; }
        public string? Numero { get; set; }
        public string? Ciudad { get; set; }
        public string? Cp { get; set; }
        public string? Habitaciones { get; set; }
        public string? Aseos { get; set; }
        public string? AseoEnsuite { get; set; }
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
        public string? PiscinaPrivada { get; set; }
        public string? Parquet { get; set; }
        public string? Antiguedad { get; set; }
        public string? Garage { get; set; }
        public string? Estado { get; set; }
        public string? DistanciaAlMar { get; set; }
        public string? Creador { get; set; }
        public string? FechaCreacion { get; set; }
        public string? FechaUltimaModificacion { get; set; }
        public string? NumeroVisitas { get; set; }
        public string? Comentario { get; set; }
        public string? Destacar { get; set; }
        public IFormFile? Foto { get; set; } // crear
        public string? ImageUrl { get; set; } // editar
        public string? ImageName { get; set; }
        public string? ImageId { get; set; }
        public string? Video { get; set; }
        public string? Ascensor { get; set; }
        public string? Trastero { get; set; }
        public string? InstalacionesDiscapacitados { get; set; }
        public string? Jardin { get; set; }
        public string? PiscinaComp { get; set; }
        public string? Columpios { get; set; }
        public string? Gym { get; set; }
        public string? Tenis { get; set; }
        public string? Padel { get; set; }
        public string? Sauna { get; set; }
        public string? Jacuzzi { get; set; }
        public string? Golf { get; set; }
        public string? VistasDespejadas { get; set; }
        public string? BajoOplantabaja { get; set; }
        public string? Puerta { get; set; }
        public string? Piso { get; set; }
        public string? Balcon { get; set; }
        public string? Alarma { get; set; }
        public string? Recepcion24_7 { get; set; }
        public string? VideoVigilancia { get; set; }
        public string? AlarmaIncendios { get; set; }
        public string? Extintores { get; set; }
        public string? AireAcondicionado { get; set; }
        public string? Calefaccion { get; set; }
        public string? PanelesSolares { get; set; }
        public string? AltaEficienciaEnergetica { get; set; }
        public string? Colegios { get; set; }
        public string? Universidades { get; set; }
        public string? Supermercados { get; set; }
        public string? Metro { get; set; }
        public string? Bus { get; set; }
        public string? SepuedeFumar { get; set; }
        public string? SeadmitenParejas { get; set; }
        public string? SeadmitenMenoresdeedad { get; set; }
        public string? SeadmitenMochileros { get; set; }
        public string? SeadmitenJubilados { get; set; }
        public string? SeadmiteLGTBI { get; set; }
        public string? PropietarioviveEnlacasa { get; set; }
        public string? PerfilCompartir { get; set; }
        public string? HabitantesActualmente { get; set; }
        public string? Ambiente { get; set; }
        public string? Gastos { get; set; }
        public string? NumeroRegistro { get; set; }
        public string? Personas { get; set; }
        public string? Camas { get; set; }
        public string? Toallas { get; set; }
        public string? Sabanas { get; set; }
        public string? Tv { get; set; }
        public string? TvCable { get; set; }
        public string? Microondas { get; set; }
        public string? Lavavajillas { get; set; }
        public string? Lavadora { get; set; }
        public string? Secadora { get; set; }
        public string? Cafetera { get; set; }
        public string? Plancha { get; set; }
        public string? Cuna { get; set; }
        public string? SecadorDePelo { get; set; }
        public string? Wifi { get; set; }
        public string? PrimeraLineaPlaya { get; set; }
        public string? Chimenea { get; set; }
        public string? MueblesJardin { get; set; }
        public string? Barbacoa { get; set; }
        public string? ValoracionesUsuarios { get; set; }
        public string? Valoraciones { get; set; }
        public string? StarRatingAverage { get; set; }
        public string? Planificacion { get; set; }
        public string? InicioDeVentas { get; set; }
        public string? InicioConstruccion { get; set; }
        public string? Mudandose { get; set; }
        public string? Mascotas { get; set; }
        public int? Fianza { get; set; }
        public string? Disponibilidad { get; set; }
        public int? EstanciaMinima { get; set; }
        public string? Model { get; set; }
    }
}