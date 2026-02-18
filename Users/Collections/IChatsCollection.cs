using Users.Models;
using Sieve.Models;

namespace Users.Collections
{
    public interface IChats
    {
        Task<List<Chat>> GetChats(string userId, string toUserId, string ViviendaId, CancellationToken cancellationToken);
        IQueryable<Chat> GetChatsQueryable();
        Task<Chat?> GetChatById(string id, CancellationToken cancellationToken);
        Task<Chat> CreateChat(Chat chat, CancellationToken cancellationToken);
        Task<Chat> UpdateChat(Chat chat, CancellationToken cancellationToken);
        Task DeleteChat(Chat chat, CancellationToken cancellationToken);
    }
}
