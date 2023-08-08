using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homes.Data;
using Homes.Models;
using Microsoft.EntityFrameworkCore;

namespace Homes.Collections
{
    public class HomeCollection : IHomeCollection
    {
        private readonly HouseDb dbc;

        public HomeCollection(HouseDb db)
        {
            dbc = db;
        }

        public async Task<List<Home>> GetAllHomes()
        {
            return await dbc.Homes.ToListAsync();
        }

        public async Task<Home?> GetHomeById(int id)
        {
            return await dbc.Homes.FindAsync(id);
        }

        public async Task NewHome(Home home)
        {
            dbc.Homes.Add(home);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateHome(Home home)
        {
            dbc.Entry(home).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public async Task DeleteHome(Home home)
        {
            dbc.Homes.Remove(home);
            await dbc.SaveChangesAsync();
        }

        public string GenerateRandomAlphanumericString()
        {
            const string chars = "1234567890";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 18)
                                                    .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}