using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homes.Models;

namespace Homes.Collections
{
    public interface IHomeCollection
    {
        public Task<Home?> GetHomeById(int id);
        public Task<Flat?> GetFlatById(int id);
        public Task<House?> GetHouseById(int id);
        public Task<HolidayRent?> GetHolidayRentById(int id);
        public Task<NewProject?> GetNewProjectById(int id);
        public Task<Room?> GetRoomById(int id);
        public Task<Home4rent?> GetHome4rentById(int id);
        public Task NewHome(Home home);
        public Task UpdateHome(Home home);
        public Task DeleteHome(Home home);
        public string GenerateRandomAlphanumericString();
        public Task<List<Home>> GetAllHomes();
        public Task<List<Flat>> GetAllFlats();
        public Task<List<House>> GetAllHouses();
        public Task<List<Home4rent>> GetAllHome4rent();
        public Task<List<NewProject>> GetAllNewProjects();
        public Task<List<Room>> GetAllRooms();
        public Task<List<HolidayRent>> GetAllHolidayRent();
        public IQueryable<Home> GetPaged();
    }
}