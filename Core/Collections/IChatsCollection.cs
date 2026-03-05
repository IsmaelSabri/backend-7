using Core.Models;

namespace Core.Collections
{
    public interface IChats
    {
        Task<List<Chat>> GetChats(Guid userId, Guid toUserId, string ViviendaId, CancellationToken cancellationToken);
        IQueryable<Chat> GetChatsQueryable();
        Task<Chat?> GetChatById(string id, CancellationToken cancellationToken);
        Task<Chat> CreateChat(Chat chat, CancellationToken cancellationToken);
        Task<Chat> UpdateChat(Chat chat, CancellationToken cancellationToken);
        Task DeleteChat(Chat chat, CancellationToken cancellationToken);
    }
}
