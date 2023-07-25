using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homes.Models;

namespace Homes.Collections
{
    public interface IHomeCollection
    {
        public Task<List<Home>> GetAllHomes();
        public Task<Home?> GetHomeById(int id);
        public Task NewHome(Home home);
        public Task UpdateHome(Home home);
        public Task DeleteHome(Home home);
        public string GenerateRandomAlphanumericString();
    }
}