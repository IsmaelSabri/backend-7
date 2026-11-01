using System.ComponentModel.DataAnnotations.Schema;
using Images.Models;
using Sieve.Attributes;

namespace Core.Models
{
    public class Home
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public Guid Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? ViviendaId { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public double Lat { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public double Lng { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Calle { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Numero { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Ciudad { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Cp { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Habitaciones { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Aseos { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? AseoEnsuite { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Superficie { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Condicion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Tipo { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PrecioInicial { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PrecioFinal { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public Decimal? PrecioHistoricoVenta { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public Decimal? PrecioHistoricoAlquiler { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PrecioAlquiler { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Duracion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Descripcion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool ArmariosEmpotrados { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Terraza { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool PiscinaPrivada { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Parquet { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Antiguedad { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Estado { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? DistanciaAlMar { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime FechaCreacion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime FechaUltimaModificacion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Model { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Video { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Amueblado { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool DireccionAproximada { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool GasNatural { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? TipoDeVia { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Distrito { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Orientacion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool AireAcondicionado { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Balcon { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Calefaccion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool PoliticaPrivacidad { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? IdCreador { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool CabinaHidromasaje { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Ascensor { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Piso { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Mascotas { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Fianza { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Disponibilidad { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? EstanciaMinima { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PrecioAlquilerInicial { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool PiscinaComp { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Trastero { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string[]? VistasDespejadas { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool InstalacionesDiscapacitados { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Jacuzzi { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool PanelesSolares { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Prototipo { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? ColorTextoDestacar { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? TourVirtual { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Bodega { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool RociadoresDeIncendios { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PlantaMasAlta { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string[]? DiscardedByUsers { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string[]? LikeMeForever { get; set; }
        [NotMapped]
        public Image[]? Images { get; set; }
        [NotMapped]
        public Image[]? Schemes { get; set; }
        [NotMapped]
        public Image? EnergyCert { get; set; }
        public ICollection<Garage>? Garages { get; set; } = [];
    }
}