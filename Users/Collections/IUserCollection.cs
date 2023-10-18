using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Dto;
using Users.Models;

namespace Users.Repositories
{
    public interface IUserCollection
    {
        Task NewUser(User user);
        Task UpdateUser(User user, string id);
        Task DeleteUser(string id);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(string id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByToken(string token);
        Task<User> GetUserByUserId(string userId);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByRefreshToken(string refreshToken);
        string GenerateRandomAlphanumericString();
        void SendWelcomeEmail(User user);
        void SendResetEmail(User user);
    }
}