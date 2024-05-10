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
            return Created("Created", true);
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
                    if (sub.Contains("model@="))
                    {
                        listTerm = sub[7..];
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