public sealed record SendMessageDto(
    Guid? UserId,
    Guid? ToUserId,
    string? ViviendaId,
    string Message,
    string State);
