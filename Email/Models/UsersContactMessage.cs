namespace Email.Models
{
    public class UsersContactMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserId { get; set; } 
        public string FromEmail { get; set; } = string.Empty;
        public string? ToEmail { get; set; } 
        public string? ToName { get; set; } 
        public string? PropertyLink { get; set; } 
        public string? PropertyThumbnail { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string? Company { get; set; } 
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
