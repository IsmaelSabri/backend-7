using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Repositories
{
    public interface IUserCollection
    {
        Task NewUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(string id);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(string id);

    }
}