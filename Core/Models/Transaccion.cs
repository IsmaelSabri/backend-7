using System.Text.Json.Serialization;
using Core.Enums;
using Sieve.Attributes;

namespace Core.Models
{
    public class Transaccion
    {
        [Sieve(CanFilter = true)]
        public Guid Id { get; set; }
        // Usuario
        [Sieve(CanFilter = true)]
        public string UsuarioId { get; set; } = string.Empty;
        [Sieve(CanFilter = true)]
        // Stripe
        public string? StripePaymentIntentId { get; set; }
        public string? StripeChargeId { get; set; }
        public string? StripeInvoiceId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter<EstadoTransaccion>))]
        public EstadoTransaccion Estado { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaPago { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; } = "EUR";
        public ICollection<LineaTransaccion> Lineas { get; set; } = new List<LineaTransaccion>();
    }
}