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
using System.Linq;
using AutoMapper.Configuration.Annotations;

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
            else if (homeDto.Model == "Other")
            {
                return Ok(await db.GetOtherById(Convert.ToInt32(id)));
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
                    case "Other":
                        home = mapper.Map<Other>(homeDto);
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

        [DisableRequestSizeLimit]
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

        [DisableRequestSizeLimit]
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

        [DisableRequestSizeLimit]
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

        [DisableRequestSizeLimit]
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

        [DisableRequestSizeLimit]
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

        [DisableRequestSizeLimit]
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

        [DisableRequestSizeLimit]
        [HttpPut("other")]
        public async Task<IActionResult> UpdateOther([FromBody] Other other)
        {
            if (other == null)
            {
                return BadRequest();
            }
            await db.UpdateOther(other);
            return Created("Modified", other);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHome(int id)
        {
            var home = await db.GetHomeById(id);
            if (home is { })
            {
                await db.DeleteHome(home);
                return Ok("Deleted");
            }
            else
            {
                return BadRequest("Cannot find model");
            }

        }

        private readonly double[] points = new double[4];
        private string CollectionModel = "";
        [HttpGet("query")]
        public IActionResult GetQuery([FromQuery] SieveModel model)
        {
            if (model.Filters.Contains("lng"))
            {
                NumberFormatInfo provider = new()
                {
                    NumberDecimalSeparator = "."
                };
                string? s = model.Filters;
                string[] subs = s.Split(',');
                for (int i = 0; i < subs.Length; i++)
                {
                    if (subs[i].Contains("lng>="))
                    {
                        points[0] = Convert.ToDouble(subs[i][5..], provider);
                        subs[i] = "";
                    }
                    else if (subs[i].Contains("lng<="))
                    {
                        points[1] = Convert.ToDouble(subs[i][5..], provider);
                        subs[i] = "";
                    }
                    else if (subs[i].Contains("lat>="))
                    {
                        points[2] = Convert.ToDouble(subs[i][5..], provider);
                        subs[i] = "";
                    }
                    else if (subs[i].Contains("lat<="))
                    {
                        points[3] = Convert.ToDouble(subs[i][5..], provider);
                        subs[i] = "";
                    }
                    else if (subs[i].Contains("model@=*"))
                    {
                        CollectionModel = subs[i][8..];
                        subs[i] = "";
                    }
                }
                var mainFilters = string.Join(",", subs.Select(p => p.ToString()).ToArray());
                model.Filters = mainFilters.Replace(",,,,", "");
                if (!string.IsNullOrEmpty(model.Filters)) // responde a los eventos del mapa con con unos criterios de filtrado dados
                {
                    var homeResult = sieveProcessor.Apply(model, db.GetBoxedHomes(points[0], points[2], points[1], points[3]).AsNoTracking());
                    homeResult = CollectionModel switch
                    {
                        "Flat" => sieveProcessor.Apply(model, db.GetBoxedFlats(points[0], points[2], points[1], points[3]).AsNoTracking()),
                        "House" => sieveProcessor.Apply(model, db.GetBoxedHouses(points[0], points[2], points[1], points[3]).AsNoTracking()),
                        "Room" => sieveProcessor.Apply(model, db.GetBoxedRooms(points[0], points[2], points[1], points[3]).AsNoTracking()),
                        "HolidayRent" => sieveProcessor.Apply(model, db.GetBoxedHolidayRent(points[0], points[2], points[1], points[3]).AsNoTracking()),
                        "NewProject" => sieveProcessor.Apply(model, db.GetBoxedNewProjects(points[0], points[2], points[1], points[3]).AsNoTracking()),
                        "Other" => sieveProcessor.Apply(model, db.GetBoxedOthers(points[0], points[2], points[1], points[3]).AsNoTracking()),
                        _ => sieveProcessor.Apply(model, db.GetBoxedHomes(points[0], points[2], points[1], points[3]).AsNoTracking()),
                    };
                    return Ok(homeResult);
                }
                else // responde a los eventos del mapa 
                {
                    return Ok(db.GetBoxedHomes(points[0], points[2], points[1], points[3]).AsNoTracking());
                }
            }
            else // consultas para solicitar modelos concretos => modelos de abajo de la jerarquía inclusive (sin coordenadas)  
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
                    homeResult = listTerm switch
                    {
                        "Flat" => sieveProcessor.Apply(model, db.GetPagedFlats().AsNoTracking()),
                        "House" => sieveProcessor.Apply(model, db.GetPagedHouses().AsNoTracking()),
                        "Room" => sieveProcessor.Apply(model, db.GetPagedRooms().AsNoTracking()),
                        "HolidayRent" => sieveProcessor.Apply(model, db.GetPagedHolidayRent().AsNoTracking()),
                        "NewProject" => sieveProcessor.Apply(model, db.GetPagedNewProjects().AsNoTracking()),
                        "Other" => sieveProcessor.Apply(model, db.GetPagedNewProjects().AsNoTracking()),
                        _ => sieveProcessor.Apply(model, db.GetPagedHomes().AsNoTracking()),
                    };
                }
                return Ok(homeResult);
            }
        }
    }
}