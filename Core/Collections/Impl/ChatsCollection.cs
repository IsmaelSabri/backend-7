using Core.Data;
using Core.Dto;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Core.Collections.Impl;

public sealed class ChatsCollection(CoreDb db, IUserBlocks blocks) : IChats
{
    private static ChatDto ToDto(Chat c, string viviendaId) =>
        new(c.Id, c.ConversationId, c.UserId, c.ToUserId, c.Message, c.Date, c.State, viviendaId);

    public async Task<List<ConversationDto>> GetConversations(Guid userId, CancellationToken ct)
    {
        var result = await db.ChatParticipants.AsNoTracking()
            .Where(p => p.UserId == userId && p.IsVisible)
            .Select(p => new
            {
                p.ConversationId,
                p.Conversation.ViviendaId,
                OtherUserId = p.Conversation.UserAId == userId ? p.Conversation.UserBId : p.Conversation.UserAId,
                LastMessage = p.Conversation.Messages
                    .Where(m => p.ClearedAt == null || m.Date > p.ClearedAt)
                    .OrderByDescending(m => m.Date).ThenByDescending(m => m.Id).FirstOrDefault(),
                UnreadCount = p.Conversation.Messages.Count(m =>
                    m.ToUserId == userId && (m.State == "Send" || m.State == "Received") &&
                    (p.ClearedAt == null || m.Date > p.ClearedAt))
            }).ToListAsync(ct);
        return result.Select(p => new ConversationDto(p.ConversationId, p.ViviendaId, p.OtherUserId,
            p.LastMessage == null ? null : ToDto(p.LastMessage, p.ViviendaId), p.UnreadCount)).ToList();
    }

    public async Task<List<ChatDto>?> GetMessages(Guid conversationId, Guid userId, CancellationToken ct)
    {
        var participant = await db.ChatParticipants.AsNoTracking()
            .Where(p => p.ConversationId == conversationId && p.UserId == userId)
            .Select(p => new { p.IsVisible, p.ClearedAt, p.Conversation.ViviendaId })
            .FirstOrDefaultAsync(ct);
        if (participant == null) return null;
        if (!participant.IsVisible) return [];
        var messages = await db.Chats.AsNoTracking()
            .Where(c => c.ConversationId == conversationId &&
                (participant.ClearedAt == null || c.Date > participant.ClearedAt))
            .OrderBy(c => c.Date).ThenBy(c => c.Id).ToListAsync(ct);
        return messages.Select(c => ToDto(c, participant.ViviendaId)).ToList();
    }

    public async Task<ChatDto> SendMessage(Guid senderId, SendChatRequest request, CancellationToken ct)
    {
        if (senderId == request.ToUserId || request.ToUserId == Guid.Empty || string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Mensaje o destinatario inválido.");
        if (await db.Users.CountAsync(u => u.Id == senderId || u.Id == request.ToUserId, ct) != 2)
            throw new KeyNotFoundException("Usuario no encontrado.");
        if (await blocks.AreUsersBlocked(senderId, request.ToUserId, ct))
            throw new ArgumentException("No se pueden enviar mensajes entre usuarios bloqueados.");
        var a = senderId.CompareTo(request.ToUserId) < 0 ? senderId : request.ToUserId;
        var b = a == senderId ? request.ToUserId : senderId;
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        Conversation? conversation;
        if (request.ConversationId is { } id)
        {
            conversation = await db.Conversations.Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserAId == a && c.UserBId == b, ct);
            if (conversation == null) throw new KeyNotFoundException("Conversación no encontrada.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.ViviendaId))
                throw new ArgumentException("Vivienda requerida.");
            conversation = await db.Conversations.Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.ViviendaId == request.ViviendaId && c.UserAId == a && c.UserBId == b, ct);
            if (conversation == null)
            {
                conversation = new Conversation { ViviendaId = request.ViviendaId, UserAId = a, UserBId = b };
                conversation.Participants.Add(new ChatParticipant { UserId = a });
                conversation.Participants.Add(new ChatParticipant { UserId = b });
                db.Conversations.Add(conversation);
                try
                {
                    await db.SaveChangesAsync(ct);
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "IX_Conversation_ViviendaId_UserAId_UserBId" })
                {
                    db.Entry(conversation).State = EntityState.Detached;
                    foreach (var participant in conversation.Participants) db.Entry(participant).State = EntityState.Detached;
                    conversation = await db.Conversations.Include(c => c.Participants)
                        .SingleAsync(c => c.ViviendaId == request.ViviendaId && c.UserAId == a && c.UserBId == b, ct);
                }
            }
        }
        var sender = conversation.Participants.SingleOrDefault(p => p.UserId == senderId);
        var recipient = conversation.Participants.SingleOrDefault(p => p.UserId == request.ToUserId);
        if (sender == null || recipient == null) throw new InvalidOperationException("Participantes inválidos.");
        sender.IsVisible = true;
        recipient.IsVisible = true;
        var chat = new Chat { ConversationId = conversation.Id, UserId = senderId, ToUserId = request.ToUserId, Message = request.Message };
        db.Chats.Add(chat);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return ToDto(chat, conversation.ViviendaId);
    }

    public async Task<bool> ClearConversation(Guid conversationId, Guid userId, CancellationToken ct)
    {
        var participant = await db.ChatParticipants.SingleOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId, ct);
        if (participant == null) return false;
        participant.ClearedAt = DateTime.UtcNow;
        participant.IsVisible = false;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<ChatDto?> UpdateMessage(Guid id, Guid userId, string state, CancellationToken ct)
    {
        var chat = await db.Chats.Include(c => c.Conversation).FirstOrDefaultAsync(c => c.Id == id && c.ToUserId == userId, ct);
        if (chat == null) return null;
        if (state != "Received" && state != "Read") throw new ArgumentException("Estado inválido.");
        if (chat.State != "Read") chat.State = state;
        await db.SaveChangesAsync(ct);
        return ToDto(chat, chat.Conversation.ViviendaId);
    }

    public async Task<List<Guid>?> MarkRead(Guid conversationId, Guid userId, CancellationToken ct)
    {
        var participant = await db.ChatParticipants.SingleOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId, ct);
        if (participant == null || !participant.IsVisible) return null;
        var messages = await db.Chats.Where(c => c.ConversationId == conversationId && c.ToUserId == userId &&
            c.State != "Read" && (participant.ClearedAt == null || c.Date > participant.ClearedAt)).ToListAsync(ct);
        foreach (var message in messages) message.State = "Read";
        await db.SaveChangesAsync(ct);
        return messages.Select(c => c.Id).ToList();
    }

    public async Task<ChatDto?> DeleteMessage(Guid id, Guid userId, CancellationToken ct)
    {
        var chat = await db.Chats.Include(c => c.Conversation)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);
        if (chat == null) return null;
        var dto = ToDto(chat, chat.Conversation.ViviendaId);
        db.Chats.Remove(chat);
        await db.SaveChangesAsync(ct);
        return dto;
    }
}
