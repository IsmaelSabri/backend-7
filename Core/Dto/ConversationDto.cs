namespace Core.Dto;

public sealed record ChatDto(
    Guid Id, Guid ConversationId, Guid UserId, Guid ToUserId,
    string Message, DateTime Date, string State, string ViviendaId);

public sealed record ConversationDto(
    Guid ConversationId, string ViviendaId, Guid OtherUserId,
    ChatDto? LastMessage, int UnreadCount);

public sealed record SendChatRequest(
    Guid? ConversationId, string? ViviendaId, Guid ToUserId, string Message);

public sealed record UpdateChatRequest(Guid Id, string State);
public sealed record ConversationRequest(Guid ConversationId);
/// <summary>Usuario al que se aplica el bloqueo.</summary>
public sealed record BlockUserRequest(Guid UserId);
