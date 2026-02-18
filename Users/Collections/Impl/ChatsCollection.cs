using Users.Data;
using Users.Models;
using Microsoft.EntityFrameworkCore;

namespace Users.Collections.Impl
{
    public class ChatsCollection(UserDb db) : IChats
    {
        private readonly UserDb db = db;

        public async Task<List<Chat>> GetChats(string userId, string toUserId, string ViviendaId, CancellationToken cancellationToken)
        {
            return await db.Chats
                .Where(p =>
                    ((p.UserId == userId && p.ToUserId == toUserId) || (p.ToUserId == userId && p.UserId == toUserId)) && 
                    p.ViviendaId == ViviendaId)
                .OrderBy(p => p.Date)
                .ToListAsync(cancellationToken);
        }

        public IQueryable<Chat> GetChatsQueryable()
        {
            return db.Chats.AsQueryable();
        }

        public async Task<Chat?> GetChatById(string id, CancellationToken cancellationToken)
        {
            return await db.Chats.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        }

        public async Task<Chat> CreateChat(Chat chat, CancellationToken cancellationToken)
        {
            await db.AddAsync(chat, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            return chat;
        }

        public async Task<Chat> UpdateChat(Chat chat, CancellationToken cancellationToken)
        {
            if (chat == null)
                throw new ArgumentNullException(nameof(chat), "Chat cannot be null");

            if (string.IsNullOrWhiteSpace(chat.Id))
                throw new ArgumentException("Chat id cannot be null or empty", nameof(chat.Id));

            var existingChat = await GetChatById(chat.Id, cancellationToken);
            if (existingChat == null)
                throw new KeyNotFoundException("Chat not found");

            existingChat.State = chat.State;
            db.Chats.Update(existingChat);
            await db.SaveChangesAsync(cancellationToken);
            return existingChat;
        }

        public async Task DeleteChat(Chat chat, CancellationToken cancellationToken)
        {
            if (chat == null)
                throw new ArgumentNullException(nameof(chat), "Chat cannot be null");

            db.Chats.Remove(chat);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
