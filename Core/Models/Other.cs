using Sieve.Attributes;

namespace Core.Models
{
    /*
    *     Prototipo: Oficina, Edificio, Negocio, Suelo, Garage, Trastero
    *     Model: other
    */
    public class Other : House
    {
        // Oficina
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Aparcamientos { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Climatizacion { get; set; } // difusores, rejillas, split, preinstalación, ninguna
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Disposicion { get; set; } // bajo, entresuelo, piso, centro comercial, pie de calle, sótano
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Distribucion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool ControlDeAccesoPersonal { get; set; } // tornos
        [Sieve(CanFilter = true, CanSort = true)]
        public bool ControlDeAccesoVehiculos { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool FalsoTecho { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool SueloTecnico { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Ascensores { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Uso { get; set; }

        // Edificio
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Inquilino { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool UsoComercial { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool UsoResidencial { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool UsoOficinas { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool UsoHotelero { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool EdificioExento { get; set; }

        // Negocio
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Nave { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Local { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? ActividadComercial { get; set; }
        [Sieve(CanFilter = true)]
        public bool HaceEsquina { get; set; }
        [Sieve(CanFilter = true)]
        public bool SalidaDeHumos { get; set; }
        [Sieve(CanFilter = true)]
        public bool Traspaso { get; set; }
        [Sieve(CanFilter = true)]
        public bool ConAlmacen { get; set; }
        [Sieve(CanFilter = true)]
        public bool ConOficina { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool LucesSalidaEmergencia { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Escaparates { get; set; }

        // Suelo
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Acceso { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Alcantarillado { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool AlumbradoPublico { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Aceras { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Agua { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Electricidad { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Calificacion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? SuperficieMinVenta { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? SuperficieMinAlquiler { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? SuperficieEdificable { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PlantasEdificables { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? NucleoUrbanoCercano { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool SinNumero { get; set; }

        // Garage
        [Sieve(CanFilter = true, CanSort = true)]
        public bool PuertaAutomatica { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public float SuperficieGarage { get; set; }


        // Trastero
        [Sieve(CanFilter = true, CanSort = true)]
        public float AlturaTrastero { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public float SuperficieTrastero { get; set; }

    }
}