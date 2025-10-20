using System.ComponentModel.DataAnnotations;
namespace Users.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public string? PaymentIntentId { get; set; }

        public string? UserId { get; set; }

        public bool IsPaid { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdatedAt { get; set;} = DateTime.UtcNow;

        public User? User  { get; set; }
    }
}
