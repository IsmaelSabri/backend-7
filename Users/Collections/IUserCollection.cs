using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Dto;
using Users.Models;

namespace Users.Collections
{
    public interface IUserCollection
    {
        Task NewUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(User user);
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserById(string id);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByToken(string token);
        Task<User?> GetUserByUserId(string userId);
        Task<User?> GetUserByUsername(string username);
        Task<User?> GetUserByRefreshToken(string refreshToken);
        IQueryable<User> GetPagedUsers();
        IQueryable<User> GetBlockYou(IQueryable<User> source, string op, string[] values);
    }
}