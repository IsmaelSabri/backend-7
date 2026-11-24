using Core.Dto;

namespace Core.Collections;

public interface IChats
{
    Task<List<ConversationDto>> GetConversations(Guid userId, CancellationToken ct);
    Task<List<ChatDto>?> GetMessages(Guid conversationId, Guid userId, CancellationToken ct);
    Task<ChatDto> SendMessage(Guid senderId, SendChatRequest request, CancellationToken ct);
    Task<bool> ClearConversation(Guid conversationId, Guid userId, CancellationToken ct);
    Task<ChatDto?> UpdateMessage(Guid id, Guid userId, string state, CancellationToken ct);
    Task<List<Guid>?> MarkRead(Guid conversationId, Guid userId, CancellationToken ct);
    Task<ChatDto?> DeleteMessage(Guid id, Guid userId, CancellationToken ct);
}
