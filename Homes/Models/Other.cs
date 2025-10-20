using System.ComponentModel.DataAnnotations;
using Sieve.Attributes;

namespace Homes.Models
{
    /*
    *     Prototipo: Oficina, Edificio, Negocio, Suelo, Garage, Trastero
    *     Model: other
    */
    public class Other : House
    {
        // Oficina
        [Sieve(CanFilter = true, CanSort = true)]
        public int Aparcamientos { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Climatizacion { get; set; } // difusores, rejillas, split, preinstalación, ninguna
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Disposicion { get; set; } // bajo, entresuelo, piso, centro comercial, pie de calle, sótano
        public string? Distribucion { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool ControlDeAccesoPersonal { get; set; } // tornos
        [Sieve(CanFilter = true, CanSort = true)]
        public bool ControlDeAccesoVehiculos { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool FalsoTecho { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool SueloTecnico { get; set; }
        public int Ascensores { get; set; }
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
        public bool EdificioExento { get; set; }

        // Negocio
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Nave { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool Local { get; set; }
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
        public bool LucesSalidaEmergencia { get; set; }
        public int Escaparates { get; set; }

        // Suelo
        public string? Acceso { get; set; }
        public bool Alcantarillado { get; set; }
        public bool AlumbradoPublico { get; set; }
        public bool Aceras { get; set; }
        public bool Agua { get; set; }
        public bool Electricidad { get; set; }
        public string? Calificacion { get; set; }
        public int SuperficieMinVenta { get; set; }
        public int SuperficieMinAlquiler { get; set; }
        public int SuperficieEdificable { get; set; }
        public int PlantasEdificables { get; set; }
        public string? NucleoUrbanoCercano { get; set; }
        public bool SinNumero { get; set; }

        // Garage
        public bool PuertaAutomatica { get; set; }
        public float SuperficieGarage { get; set; }


        // Trastero
        public float AlturaTrastero { get; set; }
        public float SuperficieTrastero { get; set; }

    }
}