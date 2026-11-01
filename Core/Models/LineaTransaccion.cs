namespace Core.Models
{
    public class LineaTransaccion
    {
        public Guid Id { get; set; }
        public Guid TransaccionId { get; set; }
        public string ExtraId { get; set; } = string.Empty;
        public string ViviendaId { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal TotalLinea { get; set; }
    }
}