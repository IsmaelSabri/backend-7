using System.ComponentModel.DataAnnotations;
using Sieve.Attributes;

namespace Homes.Models
{
    public class Home
    {
        [Key]
        public int Id { get; set; }
        [Sieve(CanFilter = true)]
        public string? ViviendaId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string? Calle { get; set; }
        public int Numero { get; set; }
        [Sieve(CanFilter = true)]
        public string? Ciudad { get; set; }
        public int Cp { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int Habitaciones { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int Aseos { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int AseoEnsuite { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int Superficie { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Condicion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Tipo { get; set; }
        public int PrecioInicial { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int PrecioFinal { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int PrecioAlquiler { get; set; }
        public string? Duracion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Descripcion { get; set; }
        public bool ArmariosEmpotrados { get; set; }
        public string? Terraza { get; set; }
        [Sieve(CanFilter = true)]
        public bool PiscinaPrivada { get; set; }
        public bool Parquet { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int Antiguedad { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int Garage { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Estado { get; set; }
        public string? DistanciaAlMar { get; set; }
        [Sieve(CanFilter = true)]
        public string? NombreCreador { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public string? NumeroVisitas { get; set; }
        public string? Comentario { get; set; }
        public string? Destacar { get; set; }
        public string? ColorDestacar { get; set; }
        [Sieve(CanFilter = true)]
        public bool Destacado { get; set; }
        [Sieve(CanFilter = true)]
        public string? Model { get; set; }
        public string? ImagesAsString { get; set; }
        public string? Video { get; set; }
        [Sieve(CanFilter = true)]
        public bool Amueblado { get; set; }
        public bool DireccionAproximada { get; set; }
        public bool GasNatural { get; set; }
        public string? Universidades { get; set; }
        public string? Metro { get; set; }
        public string? Bus { get; set; }
        public string? TipoDeVia { get; set; }
        public string? Distrito { get; set; }
        public string? Orientacion { get; set; }
        [Sieve(CanFilter = true)]
        public bool AireAcondicionado { get; set; }
        [Sieve(CanFilter = true)]
        public bool Balcon { get; set; }
        [Sieve(CanFilter = true)]
        public bool Vendido { get; set; }
        [Sieve(CanFilter = true)]
        public bool Calefaccion { get; set; }
        public bool PoliticaPrivacidad { get; set; }
        [Sieve(CanFilter = true)]
        public int ContadorLikes { get; set; }
        [Sieve(CanFilter = true)]
        public int ContadorVisitas { get; set; }
        [Sieve(CanFilter = true)]
        public string? IdCreador { get; set; }
        [Sieve(CanFilter = true)]
        public bool CabinaHidromasaje { get; set; }
        [Sieve(CanFilter = true)]
        public bool Ascensor { get; set; }
        public string? Piso { get; set; }
        public string? ProColor { get; set; }
        public string? ProImageAsString { get; set; }
        [Sieve(CanFilter = true)]
        public string? Mascotas { get; set; }
        public string? Fianza { get; set; }
        public string? Disponibilidad { get; set; }
        public string? EstanciaMinima { get; set; }
    }
}