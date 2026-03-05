using Core.Models;

namespace Core.Collections
{
    public interface IHomeCollection
    {
        public Task<Home?> GetHomeById(Guid id);
        public Task<Flat?> GetFlatById(Guid id);
        public Task<House?> GetHouseById(Guid id);
        public Task<HolidayRent?> GetHolidayRentById(Guid id);
        public Task<NewProject?> GetNewProjectById(Guid id);
        public Task<Room?> GetRoomById(Guid id);
        public Task<Other?> GetOtherById(Guid id);
        public Task NewHome(Home home);
        public Task UpdateHome(Home home);
        public Task UpdateFlat(Flat flat);
        public Task UpdateHouse(House house);
        public Task UpdateNewProject(NewProject newProject);
        public Task UpdateHolidayRent(HolidayRent holidayRent);
        public Task UpdateRoom(Room room);
        public Task UpdateOther(Other other);
        public Task DeleteHome(Home home);
        public Task<List<Home>> GetAllHomes();
        public IQueryable<Home> GetBoxedHomes(double BLng, double BLlat, double TRlng, double TRlat);
        public IQueryable<House> GetBoxedHouses(double BLng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Flat> GetBoxedFlats(double BLng, double BLlat, double TRlng, double TRlat);
        public IQueryable<NewProject> GetBoxedNewProjects(double BLng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Room> GetBoxedRooms(double BLng, double BLlat, double TRlng, double TRlat);
        public IQueryable<HolidayRent> GetBoxedHolidayRent(double BLng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Other> GetBoxedOthers(double BLng, double BLlat, double TRlng, double TRlat);
        public IQueryable<Home> GetPagedHomes();
        public IQueryable<Flat> GetPagedFlats();
        public IQueryable<House> GetPagedHouses();
        public IQueryable<HolidayRent> GetPagedHolidayRent();
        public IQueryable<Room> GetPagedRooms();
        public IQueryable<NewProject> GetPagedNewProjects();
        public IQueryable<Other> GetPagedOthers();
    }
}