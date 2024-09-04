using System.Globalization;
using AutoMapper;
using Homes.Collections;
using Homes.Data;
using Homes.Dto;
using Homes.Models;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Homes.Infrastructure;
using Sieve.Services;
using Sieve.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Homes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHomeCollection db;
        private readonly IMapper mapper;
        private readonly Cloudinary cloudinary;
        private Home home;
        private Flat flat;
        private House house;
        private HolidayRent holidayRent;
        private Room room;
        private NewProject newProject;
        private readonly SieveProcessor sieveProcessor;

        public HomeController(IMapper mapper1, IOptions<CloudinarySettings> config, HouseDb hdb, SieveProcessor _sieveProcessor)
        {
            mapper = mapper1;
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            cloudinary = new Cloudinary(account);
            db = new HomeCollection(hdb);
            home = new();
            sieveProcessor = _sieveProcessor;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllHomes()
        {
            return Ok(await db.GetAllHomes());
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> GetHomeDetails(string id, [FromBody] HomeDto homeDto)
        {
            if (homeDto == null)
            {
                return BadRequest();
            }
            else if (homeDto.Model == "Flat")
            {
                return Ok(await db.GetFlatById(Convert.ToInt32(id)));
            }
            else if (homeDto.Model == "House")
            {
                return Ok(await db.GetHouseById(Convert.ToInt32(id)));
            }
            else if (homeDto.Model == "Room")
            {
                return Ok(await db.GetRoomById(Convert.ToInt32(id)));
            }
            else if (homeDto.Model == "HolidayRent")
            {
                return Ok(await db.GetHolidayRentById(Convert.ToInt32(id)));
            }
            else if (homeDto.Model == "NewProject")
            {
                return Ok(await db.GetNewProjectById(Convert.ToInt32(id)));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateHome([FromBody] HomeDto homeDto)
        {
            if (homeDto == null)
            {
                return BadRequest();
            }
            else
            {
                switch (homeDto.Model)
                {
                    case "Flat":
                        home = mapper.Map<Flat>(homeDto);
                        break;
                    case "House":
                        home = mapper.Map<House>(homeDto);
                        break;
                    case "Room":
                        home = mapper.Map<Room>(homeDto);
                        break;
                    case "HolidayRent":
                        home = mapper.Map<HolidayRent>(homeDto);
                        break;
                    case "NewProject":
                        home = mapper.Map<NewProject>(homeDto);
                        break;
                }
                NumberFormatInfo provider = new()
                {
                    NumberDecimalSeparator = "."
                };
                NumberFormatInfo priceProvider = new()
                {
                    NumberDecimalSeparator = ","
                };
                home.Lat = Convert.ToDouble(homeDto.Lat, provider);
                home.Lng = Convert.ToDouble(homeDto.Lng, provider);
                if (!string.IsNullOrWhiteSpace(homeDto.PrecioInicial))
                {
                    home.PrecioInicial = Convert.ToInt32(homeDto.PrecioInicial, priceProvider);
                }
                if (!string.IsNullOrWhiteSpace(homeDto.PrecioFinal))
                {
                    home.PrecioFinal = Convert.ToInt32(homeDto.PrecioFinal, priceProvider);
                }
                if (!string.IsNullOrWhiteSpace(homeDto.PrecioAlquiler))
                {
                    home.PrecioAlquiler = Convert.ToInt32(homeDto.PrecioAlquiler, priceProvider);
                }
                home.FechaCreacion = DateTime.UtcNow.ToLocalTime();
                home.FechaUltimaModificacion = DateTime.UtcNow.ToLocalTime();
                home.ViviendaId = db.GenerateRandomAlphanumericString();
                var dump = ObjectDumper.Dump(home);
                Console.WriteLine(dump);
                await db.NewHome(home);
            }
            return Created("Created", home);
        }

        /*
        *      Update models
        *
        */

        [HttpPut("home")]
        public async Task<IActionResult> UpdateHome([FromBody] Home homeMod)
        {
            if (homeMod == null)
            {
                return BadRequest();
            }
            await db.UpdateHome(homeMod);
            return Created("Modified", homeMod);
        }

        [HttpPut("flat")]
        public async Task<IActionResult> UpdateFlat([FromBody] Flat flatMod)
        {
            if (flatMod == null)
            {
                return BadRequest();
            }
            await db.UpdateFlat(flatMod);
            return Created("Modified", flatMod);
        }

        [HttpPut("house")]
        public async Task<IActionResult> UpdateHouse([FromBody] House houseMod)
        {
            if (houseMod == null)
            {
                return BadRequest();
            }
            await db.UpdateHouse(houseMod);
            return Created("Modified", houseMod);
        }

        [HttpPut("room")]
        public async Task<IActionResult> UpdateRoom([FromBody] Room roomMod)
        {
            if (roomMod == null)
            {
                return BadRequest();
            }
            await db.UpdateRoom(roomMod);
            return Created("Modified", roomMod);
        }

        [HttpPut("new-project")]
        public async Task<IActionResult> UpdateNewProject([FromBody] NewProject newProjectMod)
        {
            if (newProjectMod == null)
            {
                return BadRequest();
            }
            await db.UpdateNewProject(newProjectMod);
            return Created("Modified", newProjectMod);
        }

        [HttpPut("holiday-rent")]
        public async Task<IActionResult> UpdateHolidayRent([FromBody] HolidayRent holidayRentMod)
        {
            if (holidayRentMod == null)
            {
                return BadRequest();
            }
            await db.UpdateHolidayRent(holidayRentMod);
            return Created("Modified", holidayRentMod);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHome(int id)
        {
            var home = await db.GetHomeById(id);
            await db.DeleteHome(home);
            return Ok("Deleted");
        }

        [HttpGet("query")]
        public IActionResult GetQuery([FromQuery] SieveModel model)
        {
            var homeResult = sieveProcessor.Apply(model, db.GetPagedHomes().AsNoTracking());
            if (model.Filters.Contains(','))
            {
                string? s = model.Filters;
                string[] subs = s.Split(',');
                string? listTerm = null;
                foreach (var sub in subs)
                {
                    if (sub.Contains("model@=*"))
                    {
                        listTerm = sub[8..];
                        break;
                    }
                }
                switch (listTerm)
                {
                    case "Flat":
                        homeResult = sieveProcessor.Apply(model, db.GetPagedFlats().AsNoTracking());
                        break;
                    case "House":
                        homeResult = sieveProcessor.Apply(model, db.GetPagedHouses().AsNoTracking());
                        break;
                    case "Room":
                        homeResult = sieveProcessor.Apply(model, db.GetPagedRooms().AsNoTracking());
                        break;
                    case "HolidayRent":
                        homeResult = sieveProcessor.Apply(model, db.GetPagedHolidayRent().AsNoTracking());
                        break;
                    case "NewProject":
                        homeResult = sieveProcessor.Apply(model, db.GetPagedNewProjects().AsNoTracking());
                        break;
                }
            }
            return Ok(homeResult);
        }
    }
}