namespace Core.Models;

public sealed class UserBlock
{
    public Guid BlockerUserId { get; set; }
    public Guid BlockedUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User BlockerUser { get; set; } = null!;
    public User BlockedUser { get; set; } = null!;
}
