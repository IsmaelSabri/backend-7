namespace Core.Models
{
    public sealed class PasswordResetOtp
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string Code { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; }

        public int Attempts { get; set; }
    }
}
