namespace Users.Models
{
    public class StripeWebhookEvent
    {
        public Guid Id { get; set; }
        public string? StripeEventId { get; set; }
        public string? Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public string? PayloadJson { get; set; }
    }
}