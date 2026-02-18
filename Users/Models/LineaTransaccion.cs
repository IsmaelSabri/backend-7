namespace Users.Models
{ // facturación histórica
    public class LineaTransaccion
    { // cada extracontenido que se contrata
        public Guid Id { get; set; }
        public Guid TransaccionId { get; set; }
        public Guid? ExtraContentId { get; set; }
        public string? Concepto { get; set; } // legible fiscalmente
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal IvaPorcentaje { get; set; }
        public decimal IvaImporte { get; set; }
    }
}