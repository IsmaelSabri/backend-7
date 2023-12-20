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
            else if (homeDto.Model == "Home4rent")
            {
                return Ok(await db.GetHome4rentById(Convert.ToInt32(id)));
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
                    case "Home4rent":
                        home = mapper.Map<Home4rent>(homeDto);
                        break;
                }
                NumberFormatInfo provider = new()
                {
                    NumberDecimalSeparator = "."
                };
                home.Lat = Convert.ToDouble(homeDto.Lat, provider);
                home.Lng = Convert.ToDouble(homeDto.Lng, provider);
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
            var homeResult = sieveProcessor.Apply(model, db.GetPaged().AsNoTracking());
            //Response.Headers.Add("X-Total-Count", homeResult.TotalCount.ToString());
            //Response.Headers.Add("X-Total-Pages", homeResult.TotalPages.ToString());
            return Ok(homeResult);
        }
    }
}