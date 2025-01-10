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
        public Task<Other?> GetOtherById(int id);
        public Task NewHome(Home home);
        public Task UpdateHome(Home home);
        public Task UpdateFlat(Flat flat);
        public Task UpdateHouse(House house);
        public Task UpdateNewProject(NewProject newProject);
        public Task UpdateHolidayRent(HolidayRent holidayRent);
        public Task UpdateRoom(Room room);
        public Task UpdateOther(Other other);
        public Task DeleteHome(Home home);
        public string GenerateRandomAlphanumericString();
        public Task<List<Home>> GetAllHomes();
        public IQueryable<Home> GetBoxedHomes(double BLlat, double BLlng, double TRlat, double TRlng);
        public IQueryable<Home> GetBoxedHouses(double BLlng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Home> GetBoxedFlats(double BLlng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Home> GetBoxedNewProjects(double BLlng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Home> GetBoxedRooms(double BLlng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Home> GetBoxedHolidayRent(double BLlng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Home> GetBoxedOthers(double BLlng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Home> GetPagedHomes();
        public IQueryable<Flat> GetPagedFlats();
        public IQueryable<House> GetPagedHouses();
        public IQueryable<HolidayRent> GetPagedHolidayRent();
        public IQueryable<Room> GetPagedRooms();
        public IQueryable<NewProject> GetPagedNewProjects();
        public IQueryable<Other> GetPagedOthers();
    }
}