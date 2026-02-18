using Homes.Data;
using Homes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Configuration;
using Sieve.Services;
using System.Linq.Dynamic.Core;

namespace Homes.Collections.Impl
{
    public class HomeCollection : IHomeCollection, ISieveCustomFilterMethods
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

        public async Task<Other?> GetOtherById(int id)
        {
            return await dbc.Others.FindAsync(id);
        }

        public async Task NewHome(Home home)
        {
            dbc.Homes.Add(home);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateHome(Home home)
        {
            dbc.Homes.Entry(home).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateFlat(Flat flat)
        {
            dbc.Flats.Entry(flat).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateHouse(House house)
        {
            dbc.Houses.Entry(house).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateNewProject(NewProject newProject)
        {
            dbc.NewProjects.Entry(newProject).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateHolidayRent(HolidayRent holidayRent)
        {
            dbc.HolidayRents.Entry(holidayRent).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateRoom(Room room)
        {
            dbc.Rooms.Entry(room).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateOther(Other other)
        {
            dbc.Others.Entry(other).State = EntityState.Modified;
            await dbc.SaveChangesAsync();
        }

        public async Task DeleteHome(Home home)
        {
            dbc.Homes.Remove(home);
            await dbc.SaveChangesAsync();
        }

        public IQueryable<Home> GetBoxedHomes(double BLlng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Homes.Where(x => x.Lng >= BLlng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<House> GetBoxedHouses(double BLlng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Houses.Where(x => x.Lng >= BLlng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Flat> GetBoxedFlats(double BLlng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Flats.Where(x => x.Lng >= BLlng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<NewProject> GetBoxedNewProjects(double BLlng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.NewProjects.Where(x => x.Lng >= BLlng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Room> GetBoxedRooms(double BLlng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Rooms.Where(x => x.Lng >= BLlng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<HolidayRent> GetBoxedHolidayRent(double BLlng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.HolidayRents.Where(x => x.Lng >= BLlng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Other> GetBoxedOthers(double BLlng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Others.Where(x => x.Lng >= BLlng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
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

        public IQueryable<Other> GetPagedOthers()
        {
            return dbc.Others.AsQueryable();
        }
    }
}