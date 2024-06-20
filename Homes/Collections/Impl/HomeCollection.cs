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

        public IQueryable<Home> GetPagedHomes()
        {
            return dbc.Homes.AsQueryable();
        }

        public IQueryable<Flat> GetPagedFlats()
        {
            return dbc.Flats.AsQueryable();
        }

        public IQueryable<House> GetPagedHouses()
        {
            return dbc.Houses.AsQueryable();
        }

        public IQueryable<HolidayRent> GetPagedHolidayRent()
        {
            return dbc.HolidayRents.AsQueryable();
        }

        public IQueryable<Room> GetPagedRooms()
        {
            return dbc.Rooms.AsQueryable();
        }

        public IQueryable<NewProject> GetPagedNewProjects()
        {
            return dbc.NewProjects.AsQueryable();
        }
    }
}