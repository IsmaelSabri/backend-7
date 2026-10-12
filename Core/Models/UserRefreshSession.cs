namespace Core.Models;

public class UserRefreshSession
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
