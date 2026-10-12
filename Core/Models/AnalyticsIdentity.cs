namespace Core.Models;

public class AnalyticsIdentity
{
    public Guid Id { get; set; }
    public string AnonymousId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public DateTime FirstSeenAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
    public bool Resolved { get; set; }
}
