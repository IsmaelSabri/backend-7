namespace Users.Models
{
    public class Factura
    {
        public Guid Id { get; set; }

        public string? Numero { get; set; } // Secuencial
        public DateTime FechaEmision { get; set; }

        public Guid TransaccionId { get; set; }

        public string? NombreFiscal { get; set; }
        public string? Cif { get; set; }
    }
}