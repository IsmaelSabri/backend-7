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
        public bool Aparcamientos { get; set; }
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
        public bool UsoExclusivoOficina { get; set; }
        public bool UsoMixtoOficina { get; set; }

        // Edificio
        [Sieve(CanFilter = true, CanSort = true)]
        public bool EdificioExclusivoOficinas { get; set; }

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
        public bool LucesSalidaEmergencia {get; set;}
        public int Escaparates {get; set;}

        // Suelo
        [Sieve(CanFilter = true)]
        public bool Urbano { get; set; }
        [Sieve(CanFilter = true)]
        public bool Urbanizable { get; set; }
        [Sieve(CanFilter = true)]
        public bool NoUrbanizable { get; set; }

        // Garage
        [Sieve(CanFilter = true)]
        public bool PlazaParaCoche { get; set; }
        [Sieve(CanFilter = true)]
        public bool PlazaParaMoto { get; set; }

        // Trastero

        // Edificio
    }
}