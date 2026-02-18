using System.ComponentModel.DataAnnotations;
namespace Users.Models
{
    public class ExtraContent // se persiste
    // derecho de uso. Aquí vive la caducidad
    {
        [Key]
        public Guid Id { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public string? UsuarioId { get; set; }
        public string? ViviendaId { get; set; }
        public string? ExtraId { get; set; }
        public string? LineaTransaccionId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
