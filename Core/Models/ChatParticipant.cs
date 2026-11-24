namespace Core.Models;

public sealed class ChatParticipant
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime? ClearedAt { get; set; }
    public bool IsVisible { get; set; } = true;
    public Conversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}
