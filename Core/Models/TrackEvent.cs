namespace Core.Models;

public class TrackEvent
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string AnonymousId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? HomeId { get; set; }
    public string? Channel { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? Metadata { get; set; }
}