using Sieve.Attributes;
using Users.Enums;
namespace Users.Models
{
    public class Transaccion
    {
        [Sieve(CanFilter = true)]
        public Guid Id { get; set; }
        // Usuario
        [Sieve(CanFilter = true)]
        public string? UsuarioId { get; set; }
        [Sieve(CanFilter = true)]
        // Stripe
        public string? StripePaymentIntentId { get; set; }
        public string? StripeChargeId { get; set; }
        // si se utiliza facturación periódica/recurrente, 
        // como los son las suscripciones(basic, silver, gold)
        public string? StripeInvoiceId { get; set; } 
        public string? StripeSubscriptionId { get; set; }
        // Estado
        public EstadoTransaccion Estado { get; set; }
        // Fechas
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaPago { get; set; }
        public DateTime? FechaReembolso { get; set; }
        public decimal? ImporteReembolsado { get; set; }
        // Importes (SIEMPRE desde Stripe)
        public decimal BaseImponible { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; } = "EUR";
    }
}