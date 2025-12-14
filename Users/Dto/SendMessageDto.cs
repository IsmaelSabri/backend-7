public sealed record SendMessageDto(
    string? UserId,
    string? ToUserId,
    string? ViviendaId,
    string Message,
    string State);
