namespace Core.Models;

public sealed class Chat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public Guid ToUserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string State { get; set; } = "Send";
    public Conversation Conversation { get; set; } = null!;
}
