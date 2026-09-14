namespace Email.Models
{
    public class ContactMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Reference { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "pending";
        public string? AnsweredBy { get; set; }
        public DateTime? AnsweredAt { get; set; } = null;
    }
}
