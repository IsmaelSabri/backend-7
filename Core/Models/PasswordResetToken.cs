namespace Core.Models
{
    public sealed class PasswordResetToken
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}