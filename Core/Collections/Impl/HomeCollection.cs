using Core.Data;
using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Core.Collections.Impl
{
    public class HomeCollection : IHomeCollection
    {
        private readonly CoreDb dbc;
        private readonly IImageService _imageService;

        public HomeCollection(CoreDb db, IImageService imageService)
        {
            dbc = db;
            _imageService = imageService;
        }

        public async Task<List<Home>> GetAllHomes()
        {
            var homes = await dbc.Homes.Include(h => h.Garages).ToListAsync();

            // Inicializar imágenes para todos los homes
            foreach (var home in homes)
            {
                await InitializeImagesAsync(home);
            }

            return homes;
        }

        public async Task<Home?> GetHomeById(Guid id)
        {
            var home = await dbc.Homes.Include(x => x.Garages).FirstOrDefaultAsync(x => x.Id == id);
            if (home != null)
            {
                await InitializeImagesAsync(home);
            }
            return home;
        }

        public async Task<Home?> GetHomeByViviendaId(string id)
        {
            var home = await dbc.Homes.Include(x => x.Garages).FirstOrDefaultAsync(x => x.ViviendaId == id);
            if (home != null)
            {
                await InitializeImagesAsync(home);
            }
            return home;
        }

        public async Task<Flat?> GetFlatByViviendaId(string id)
        {
            var flat = await dbc.Flats.Include(x => x.Garages).FirstOrDefaultAsync(x => x.ViviendaId == id);
            if (flat != null)
            {
                await InitializeImagesAsync(flat);
            }
            return flat;
        }

        public async Task<House?> GetHouseByViviendaId(string id)
        {
            var house = await dbc.Houses.Include(x => x.Garages).FirstOrDefaultAsync(x => x.ViviendaId == id);
            if (house != null)
            {
                await InitializeImagesAsync(house);
            }
            return house;
        }

        public async Task<HolidayRent?> GetHolidayRentByViviendaId(string id)
        {
            var holidayRent = await dbc.HolidayRents.Include(x => x.Garages).FirstOrDefaultAsync(x => x.ViviendaId == id);
            if (holidayRent != null)
            {
                await InitializeImagesAsync(holidayRent);
            }
            return holidayRent;
        }

        public async Task<NewProject?> GetNewProjectByViviendaId(string id)
        {
            var newProject = await dbc.NewProjects.Include(x => x.Garages).FirstOrDefaultAsync(x => x.ViviendaId == id);
            if (newProject != null)
            {
                await InitializeImagesAsync(newProject);
            }
            return newProject;
        }

        public async Task<Room?> GetRoomByViviendaId(string id)
        {
            var room = await dbc.Rooms.Include(x => x.Garages).FirstOrDefaultAsync(x => x.ViviendaId == id);
            if (room != null)
            {
                await InitializeImagesAsync(room);
            }
            return room;
        }

        public async Task<Other?> GetOtherByViviendaId(string id)
        {
            var other = await dbc.Others.Include(x => x.Garages).FirstOrDefaultAsync(x => x.ViviendaId == id);
            if (other != null)
            {
                await InitializeImagesAsync(other);
            }
            return other;
        }

        public async Task<Home?> GetStoredHomePrices(Guid id)
        {
            return await dbc.Homes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Flat?> GetFlatById(Guid id)
        {
            var flat = await dbc.Flats.FindAsync(id);
            if (flat != null)
            {
                await InitializeImagesAsync(flat);
            }
            return flat;
        }

        public async Task<House?> GetHouseById(Guid id)
        {
            var house = await dbc.Houses.Include(x => x.Garages).FirstOrDefaultAsync(x => x.Id == id);
            if (house != null)
            {
                await InitializeImagesAsync(house);
            }
            return house;
        }

        public async Task<HolidayRent?> GetHolidayRentById(Guid id)
        {
            var holidayRent = await dbc.HolidayRents.Include(x => x.Garages).FirstOrDefaultAsync(x => x.Id == id);
            if (holidayRent != null)
            {
                await InitializeImagesAsync(holidayRent);
            }
            return holidayRent;
        }

        public async Task<NewProject?> GetNewProjectById(Guid id)
        {
            var newProject = await dbc.NewProjects.Include(x => x.Garages).FirstOrDefaultAsync(x => x.Id == id);
            if (newProject != null)
            {
                await InitializeImagesAsync(newProject);
            }
            return newProject;
        }

        public async Task<Room?> GetRoomById(Guid id)
        {
            var room = await dbc.Rooms.Include(x => x.Garages).FirstOrDefaultAsync(x => x.Id == id);
            if (room != null)
            {
                await InitializeImagesAsync(room);
            }
            return room;
        }

        public async Task<Other?> GetOtherById(Guid id)
        {
            var other = await dbc.Others.Include(x => x.Garages).FirstOrDefaultAsync(x => x.Id == id);
            if (other != null)
            {
                await InitializeImagesAsync(other);
            }
            return other;
        }

        public async Task NewHome(Home home)
        {
            dbc.Homes.Add(home);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateHome(Home home)
        {
            dbc.Homes.Update(home);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateFlat(Flat flat)
        {
            dbc.Flats.Update(flat);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateHouse(House house)
        {
            dbc.Houses.Update(house);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateNewProject(NewProject newProject)
        {
            dbc.NewProjects.Update(newProject);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateHolidayRent(HolidayRent holidayRent)
        {
            dbc.HolidayRents.Update(holidayRent);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateRoom(Room room)
        {
            dbc.Rooms.Update(room);
            await dbc.SaveChangesAsync();
        }

        public async Task UpdateOther(Other other)
        {
            dbc.Others.Update(other);
            await dbc.SaveChangesAsync();
        }

        public async Task DeleteHome(Home home)
        {
            dbc.Homes.Remove(home);
            await dbc.SaveChangesAsync();
        }

        public IQueryable<Home> GetBoxedHomes(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Homes.Include(x => x.Garages).Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<House> GetBoxedHouses(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Houses.Include(x => x.Garages).Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Flat> GetBoxedFlats(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Flats.Include(x => x.Garages).Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<NewProject> GetBoxedNewProjects(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.NewProjects.Include(x => x.Garages).Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Room> GetBoxedRooms(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Rooms.Include(x => x.Garages).Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<HolidayRent> GetBoxedHolidayRent(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.HolidayRents.Include(x => x.Garages).Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Other> GetBoxedOthers(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Others.Include(x => x.Garages).Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Home> GetPagedHomes()
        {
            return dbc.Homes.Include(x => x.Garages).AsQueryable();
        }

        public IQueryable<Flat> GetPagedFlats()
        {
            return dbc.Flats.Include(x => x.Garages).AsQueryable();
        }

        public IQueryable<House> GetPagedHouses()
        {
            return dbc.Houses.Include(x => x.Garages).AsQueryable();
        }

        public IQueryable<HolidayRent> GetPagedHolidayRent()
        {
            return dbc.HolidayRents.Include(x => x.Garages).AsQueryable();
        }

        public IQueryable<Room> GetPagedRooms()
        {
            return dbc.Rooms.Include(x => x.Garages).AsQueryable();
        }

        public IQueryable<NewProject> GetPagedNewProjects()
        {
            return dbc.NewProjects.Include(x => x.Garages).AsQueryable();
        }

        public IQueryable<Other> GetPagedOthers()
        {
            return dbc.Others.Include(x => x.Garages).AsQueryable();
        }

        public async Task<IQueryable<Home>> GetHomesByLikeMeForever(string userid)
        {
            var likedHomes = await dbc.Homes
                .Include(h => h.Garages)
                .Where(ul => ul.LikeMeForever.Contains(userid))
                .ToListAsync();

            // Inicializar imágenes para los homes obtenidos
            foreach (var home in likedHomes)
            {
                await InitializeImagesAsync(home);
            }

            return likedHomes.AsQueryable();
        }

        public async Task<List<Home>> GetDiscardedHomes(string userid)
        {
            var discardedHomes = await dbc.Homes
                .Include(h => h.Garages)
                .Where(ul => ul.DiscardedByUsers.Contains(userid))
                .ToListAsync();

            // Inicializar imágenes para los homes obtenidos
            foreach (var home in discardedHomes)
            {
                await InitializeImagesAsync(home);
            }

            return discardedHomes;
        }

        private async Task InitializeImagesAsync(Home home)
        {
            if (home == null || string.IsNullOrEmpty(home.ViviendaId))
                return;

            try
            {
                var imagesDto = await _imageService.GetHomeImagesByViviendaIdAsync(home.ViviendaId);
                if (imagesDto != null)
                {
                    if (imagesDto.Images != null && imagesDto.Images.Length > 0)
                    {
                        home.Images = imagesDto.Images;
                    }

                    if (imagesDto.Schemes != null && imagesDto.Schemes.Length > 0)
                    {
                        home.Schemes = imagesDto.Schemes;
                    }

                    if (imagesDto.EnergyCert != null)
                    {
                        home.EnergyCert = imagesDto.EnergyCert;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log o manejar el error sin romper el flujo
                Console.WriteLine($"Error initializing images for home {home.ViviendaId}: {ex.Message}");
            }
        }

    }
}