using Core.Models;
using Images.Models;

namespace Core.Dto
{
    /// <summary>
    /// DTO plano que agrupa todos los atributos de la clase Home del cliente (home.ts).
    /// AutoMapper lo proyecta sobre Home, Flat, House, Room, HolidayRent, NewProject u Other según Model.
    /// </summary>
    public class HomeDto
    {
        public Guid Id { get; set; }
        public string? ViviendaId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string? Calle { get; set; }
        public int? Numero { get; set; }
        public string? Ciudad { get; set; }
        public int? Cp { get; set; }
        public int? Habitaciones { get; set; }
        public int? Aseos { get; set; }
        public int? AseoEnsuite { get; set; }
        public int? Superficie { get; set; }
        public string? Condicion { get; set; }
        public string? Tipo { get; set; }
        public int? PrecioInicial { get; set; }
        public int? PrecioFinal { get; set; }
        public decimal? PrecioHistoricoVenta { get; set; }
        public int? PrecioAlquiler { get; set; }
        public int? PrecioAlquilerInicial { get; set; }
        public decimal? PrecioHistoricoAlquiler { get; set; }
        public string? Duracion { get; set; }
        public string? Descripcion { get; set; }
        public bool ArmariosEmpotrados { get; set; }
        public bool Terraza { get; set; }
        public bool PiscinaPrivada { get; set; }
        public bool Parquet { get; set; }
        public int? Antiguedad { get; set; }
        public string? Estado { get; set; }
        public string? DistanciaAlMar { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public string? Model { get; set; }
        public Image[]? Images { get; set; }
        public Image[]? Schemes { get; set; }
        public Image? EnergyCert { get; set; }
        public string? Video { get; set; }
        public bool Amueblado { get; set; }
        public bool DireccionAproximada { get; set; }
        public bool GasNatural { get; set; }
        public string? TipoDeVia { get; set; }
        public string? Distrito { get; set; }
        public string? Orientacion { get; set; }
        public bool Vendido { get; set; }
        public bool Balcon { get; set; }
        public bool AireAcondicionado { get; set; }
        public bool Calefaccion { get; set; }
        public int? PlantaMasAlta { get; set; }
        public bool PoliticaPrivacidad { get; set; }
        public string? IdCreador { get; set; }
        public string? OrganizationId { get; set; }
        public bool CabinaHidromasaje { get; set; }
        public bool Ascensor { get; set; }
        public int? Piso { get; set; }
        public string? Mascotas { get; set; }
        public string? Fianza { get; set; }
        public string? Disponibilidad { get; set; }
        public string? EstanciaMinima { get; set; }
        public string[]? LikeMeForever { get; set; }
        public string[]? DiscardedByUsers { get; set; }
        public bool PiscinaComp { get; set; }
        public string[]? VistasDespejadas { get; set; }
        public bool InstalacionesDiscapacitados { get; set; }
        public bool Trastero { get; set; }
        public bool PanelesSolares { get; set; }
        public bool Jacuzzi { get; set; }
        public string? Prototipo { get; set; }
        public string? ColorTextoDestacar { get; set; }
        public string? TourVirtual { get; set; }
        public bool Bodega { get; set; }
        public bool RociadoresDeIncendios { get; set; }

        // House
        public bool Alarma { get; set; }
        public bool Recepcion24_7 { get; set; }
        public bool VideoVigilancia { get; set; }
        public bool AlarmaIncendios { get; set; }
        public bool Extintores { get; set; }
        public string? Consumo { get; set; }
        public string? Emisiones { get; set; }
        public bool Aerotermia { get; set; }
        public bool VentilacionCruzada { get; set; }
        public bool DobleAcristalamiento { get; set; }
        public bool GeneradorEmergencia { get; set; }
        public bool Golf { get; set; }
        public bool Gym { get; set; }
        public int? Alturas { get; set; }
        public int? TotalViviendasConstruidas { get; set; }

        // Flat
        public bool BajoOplantabaja { get; set; }
        public bool Jardin { get; set; }
        public bool Columpios { get; set; }
        public bool Tenis { get; set; }
        public bool Padel { get; set; }
        public bool Sauna { get; set; }
        public bool ZonaDeOcio { get; set; }
        public bool VideoPortero { get; set; }

        // Room
        public bool SepuedeFumar { get; set; }
        public bool SeadmitenParejas { get; set; }
        public bool SeadmitenMenoresdeedad { get; set; }
        public bool SeadmitenMochileros { get; set; }
        public bool SeadmitenJubilados { get; set; }
        public bool SeadmiteLGTBI { get; set; }
        public string? PerfilCompartir { get; set; }
        public string? HabitantesActualmente { get; set; }
        public bool PropietarioviveEnlacasa { get; set; }
        public string? Ambiente { get; set; }
        public string? Gastos { get; set; }

        // NewProject
        public string? NombreProyecto { get; set; }
        public string? EstadoConstruccion { get; set; }
        public string[]? Tipos { get; set; }
        public int? PorcentajeTerminado { get; set; }
        public int? PorcentajeVendido { get; set; }
        public int? HabitacionesDesde { get; set; }
        public int? HabitacionesHasta { get; set; }
        public int? SuperficieDesde { get; set; }
        public int? SuperficieHasta { get; set; }
        public int? FinDeObra { get; set; }

        // HolidayRent
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
        public int? Valoraciones { get; set; }
        public int? StarRatingAverage { get; set; }

        // Other: Oficina
        public int? Aparcamientos { get; set; }
        public string? Climatizacion { get; set; }
        public string? Disposicion { get; set; }
        public string? Distribucion { get; set; }
        public bool ControlDeAccesoPersonal { get; set; }
        public bool ControlDeAccesoVehiculos { get; set; }
        public bool FalsoTecho { get; set; }
        public bool SueloTecnico { get; set; }
        public string? Uso { get; set; }

        // Other: Edificio
        public int? Ascensores { get; set; }
        public bool Inquilino { get; set; }
        public bool UsoResidencial { get; set; }
        public bool UsoComercial { get; set; }
        public bool UsoOficinas { get; set; }
        public bool UsoHotelero { get; set; }
        public bool EdificioExento { get; set; }

        // Other: Negocio
        public bool Nave { get; set; }
        public bool Local { get; set; }
        public string? ActividadComercial { get; set; }
        public bool HaceEsquina { get; set; }
        public bool SalidaDeHumos { get; set; }
        public bool Traspaso { get; set; }
        public bool ConAlmacen { get; set; }
        public bool ConOficina { get; set; }
        public bool LucesSalidaEmergencia { get; set; }
        public int? Escaparates { get; set; }

        // Other: Suelo
        public string? Acceso { get; set; }
        public bool Alcantarillado { get; set; }
        public bool AlumbradoPublico { get; set; }
        public bool Aceras { get; set; }
        public bool Agua { get; set; }
        public bool Electricidad { get; set; }
        public int? SuperficieMinVenta { get; set; }
        public int? SuperficieMinAlquiler { get; set; }
        public int? SuperficieEdificable { get; set; }
        public int? PlantasEdificables { get; set; }
        public string? Calificacion { get; set; }
        public string? NucleoUrbanoCercano { get; set; }
        public bool SinNumero { get; set; }

        // Other: Garage
        public bool PuertaAutomatica { get; set; }
        public float SuperficieGarage { get; set; }

        // Other: Trastero
        public float AlturaTrastero { get; set; }
        public float SuperficieTrastero { get; set; }

        public ICollection<Garage>? Garages { get; set; }
        public ICollection<ExtraContent>? Extras { get; set; }
    }
}
