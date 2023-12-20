using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Sieve.Attributes;

namespace Homes.Models
{
    public class Home
    {
        [Key]
        public int Id { get; set; }
        public string? ViviendaId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string? Calle { get; set; }
        public int Numero { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Ciudad { get; set; }
        public int Cp { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Habitaciones { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Aseos { get; set; }
        public string? AseoEnsuite { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Superficie { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Condicion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Tipo { get; set; }
        public string? PrecioInicial { get; set; }
        public string? Descuento { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? PrecioFinal { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? PrecioAlquiler { get; set; }
        public string? Duracion { get; set; }
        public string? Descripcion { get; set; }
        public bool ArmariosEmpotrados { get; set; }
        public string? Terraza { get; set; }
        public bool PiscinaPrivada { get; set; }
        public bool Parquet { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Antiguedad { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Garage { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Estado { get; set; }
        public string? DistanciaAlMar { get; set; }
        public string? Creador { get; set; }
        public string? NombreCreador { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public string? NumeroVisitas { get; set; }
        public string? Comentario { get; set; }
        public string? Destacar { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Model { get; set; }
        public string? ImagesAsString { get; set; }
        public string? Video { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Amueblado { get; set; }
        public string? StreetView { get; set; }
        public bool DireccionAproximada { get; set; }
        public bool GasNatural { get; set; }
        public string? Universidades { get; set; }
        public string? Metro { get; set; }
        public string? Bus { get; set; }
        public string? TipoDeVia { get; set; }
        public string? Distrito { get; set; }
        public string? Orientacion { get; set; }
        public string? VideoPortero { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? PlantaMasAlta { get; set; }
        public string? ZonaDeOcio { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool AireAcondicionado { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Balcon { get; set; }
    }
}