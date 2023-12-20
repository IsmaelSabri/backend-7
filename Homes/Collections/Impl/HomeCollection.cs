using Homes.Data;
using Homes.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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

        public async Task<List<Flat>> GetAllFlats()
        {
            return await dbc.Flats.ToListAsync();
        }

        public async Task<List<House>> GetAllHouses()
        {
            return await dbc.Houses.ToListAsync();
        }

        public async Task<List<Home4rent>> GetAllHome4rent()
        {
            return await dbc.Home4Rents.ToListAsync();
        }

        public async Task<List<NewProject>> GetAllNewProjects()
        {
            return await dbc.NewProjects.ToListAsync();
        }

        public async Task<List<Room>> GetAllRooms()
        {
            return await dbc.Rooms.ToListAsync();
        }

        public async Task<List<HolidayRent>> GetAllHolidayRent()
        {
            return await dbc.HolidayRents.ToListAsync();
        }

        public async Task<Home?> GetHomeById(int id)
        {
            return await dbc.Homes.FindAsync(id);
        }

        public async Task<Flat?> GetFlatById(int id)
        {
            return await dbc.Flats.FindAsync(id);
        }

        public async Task<House?> GetHouseById(int id)
        {
            return await dbc.Houses.FindAsync(id);
        }

        public async Task<HolidayRent?> GetHolidayRentById(int id)
        {
            return await dbc.HolidayRents.FindAsync(id);
        }

        public async Task<NewProject?> GetNewProjectById(int id)
        {
            return await dbc.NewProjects.FindAsync(id);
        }

        public async Task<Room?> GetRoomById(int id)
        {
            return await dbc.Rooms.FindAsync(id);
        }

        public async Task<Home4rent?> GetHome4rentById(int id)
        {
            return await dbc.Home4Rents.FindAsync(id);
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

        public IQueryable<Home> GetPaged()
        {
            return dbc.Homes.AsQueryable();
        }
    }
}