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
            var homes = await dbc.Homes.ToListAsync();
            
            // Inicializar imágenes para todos los homes
            foreach (var home in homes)
            {
                await InitializeImagesAsync(home);
            }
            
            return homes;
        }

        public async Task<Home?> GetHomeById(Guid id)
        {
            var home = await dbc.Homes.FindAsync(id);
            if (home != null)
            {
                await InitializeImagesAsync(home);
            }
            return home;
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
            var house = await dbc.Houses.FindAsync(id);
            if (house != null)
            {
                await InitializeImagesAsync(house);
            }
            return house;
        }

        public async Task<HolidayRent?> GetHolidayRentById(Guid id)
        {
            var holidayRent = await dbc.HolidayRents.FindAsync(id);
            if (holidayRent != null)
            {
                await InitializeImagesAsync(holidayRent);
            }
            return holidayRent;
        }

        public async Task<NewProject?> GetNewProjectById(Guid id)
        {
            var newProject = await dbc.NewProjects.FindAsync(id);
            if (newProject != null)
            {
                await InitializeImagesAsync(newProject);
            }
            return newProject;
        }

        public async Task<Room?> GetRoomById(Guid id)
        {
            var room = await dbc.Rooms.FindAsync(id);
            if (room != null)
            {
                await InitializeImagesAsync(room);
            }
            return room;
        }

        public async Task<Other?> GetOtherById(Guid id)
        {
            var other = await dbc.Others.FindAsync(id);
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

        public IQueryable<Home> GetBoxedHomes(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Homes.Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<House> GetBoxedHouses(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Houses.Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Flat> GetBoxedFlats(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Flats.Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<NewProject> GetBoxedNewProjects(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.NewProjects.Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Room> GetBoxedRooms(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Rooms.Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<HolidayRent> GetBoxedHolidayRent(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.HolidayRents.Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
        }

        public IQueryable<Other> GetBoxedOthers(double BLng, double BLlat, double TRlng, double TRlat)
        {
            return dbc.Others.Where(x => x.Lng >= BLng && x.Lng <= TRlng && x.Lat >= BLlat && x.Lat <= TRlat);
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
                System.Console.WriteLine($"Error initializing images for home {home.ViviendaId}: {ex.Message}");
            }
        }

    }
}