using Core.Models;

namespace Core.Collections
{
    public interface IUserCollection
    {
        Task NewUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(User user);
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByToken(string token);
        Task<User?> GetUserByUserId(string userId);
        Task<User?> GetUserByUsername(string username);
        Task<User?> GetUserByRefreshToken(string refreshToken);
        IQueryable<User> GetPagedUsers();
        IQueryable<User> GetBlockYou(IQueryable<User> source, string op, string[] values);
    }
}