using System.Globalization;
using AutoMapper;
using Core.Collections.Impl;
using Core.Collections;
using Core.Data;
using Core.Dto;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Core.Extensions;
using Sieve.Services;
using Sieve.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.NetworkInformation;
using Core.Services;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHomeCollection db;
        private readonly IMapper mapper;
        private Home home;
        private readonly SieveProcessor sieveProcessor;
        private readonly IImageService _imageService;

        public HomeController(IMapper mapper1, CoreDb hdb, SieveProcessor _sieveProcessor, IImageService imageService)
        {
            mapper = mapper1;
            db = new HomeCollection(hdb, imageService);
            home = new();
            sieveProcessor = _sieveProcessor;
            _imageService = imageService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllHomes()
        {
            return Ok(await db.GetAllHomes());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHomeDetails(Guid id)
        {
            var baseHome = await db.GetHomeById(id);
            if (baseHome == null)
            {
                return NotFound();
            }

            return baseHome.Model switch
            {
                "Flat" => Ok(await db.GetFlatById(id)),
                "House" => Ok(await db.GetHouseById(id)),
                "Room" => Ok(await db.GetRoomById(id)),
                "HolidayRent" => Ok(await db.GetHolidayRentById(id)),
                "NewProject" => Ok(await db.GetNewProjectById(id)),
                "Other" => Ok(await db.GetOtherById(id)),
                _ => Ok(baseHome)
            };
        }

        [HttpPost("ping/{url}")]
        public async Task<IActionResult> ImageCallback(string url)
        {
            Ping pingsender = new();
            try
            {
                PingReply reply = await pingsender.SendPingAsync(url);
                if (reply.Status == IPStatus.Success)
                {
                    return Ok("Success!");
                }
                else
                {
                    return BadRequest("Service unavailable");
                }
            }
            catch (Exception)
            {
                return BadRequest("Something wrong");
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
                        var other = mapper.Map<Other>(homeDto);
                        NumberFormatInfo floatPoint = new()
                        {
                            NumberDecimalSeparator = "."
                        };
                        if (!string.IsNullOrWhiteSpace(homeDto.SuperficieGarage))
                        {
                            other.SuperficieGarage = Convert.ToSingle(homeDto.SuperficieGarage, floatPoint);
                        }
                        if (!string.IsNullOrWhiteSpace(homeDto.SuperficieTrastero))
                        {
                            other.SuperficieTrastero = Convert.ToSingle(homeDto.SuperficieTrastero, floatPoint);
                        }
                        if (!string.IsNullOrWhiteSpace(homeDto.AlturaTrastero))
                        {
                            other.AlturaTrastero = Convert.ToSingle(homeDto.AlturaTrastero, floatPoint);
                        }
                        home = other;
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
                var dump = ObjectDumper.Dump(home);
                Console.WriteLine(dump);
                // Normalize string properties: replace null strings with empty string before persisting
                home.NormalizeNullStrings();
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
            var dump = ObjectDumper.Dump(flatMod);
            Console.WriteLine(dump);
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
        public async Task<IActionResult> DeleteHome(Guid id)
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
        public async Task<IActionResult> GetQuery([FromQuery] SieveModel model)
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
                    // Materializar el resultado y componer con imágenes
                    var homes = homeResult.Cast<Home>().ToList();
                    var homesWithImages = await ComposeWithImagesAsync(homes);
                    return Ok(homesWithImages);
                }
                else // responde a los eventos del mapa 
                {
                    var homes = await db.GetBoxedHomes(points[0], points[2], points[1], points[3]).AsNoTracking().ToListAsync();
                    var homesWithImages = await ComposeWithImagesAsync(homes);
                    return Ok(homesWithImages);
                }
            }
            else // consultas para solicitar modelos concretos => modelos de abajo de la jerarquía inclusive (sin coordenadas)  
            {
                var homeResult = sieveProcessor.Apply(model, db.GetPagedHomes().AsNoTracking());
                var dump = ObjectDumper.Dump(model);
                Console.WriteLine(dump);
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
                        "Other" => sieveProcessor.Apply(model, db.GetPagedOthers().AsNoTracking()),
                        _ => sieveProcessor.Apply(model, db.GetPagedHomes().AsNoTracking()),
                    };
                }
                // Materializar el resultado y componer con imágenes
                var homes = homeResult.Cast<Home>().ToList();
                var homesWithImages = await ComposeWithImagesAsync(homes);
                return Ok(homesWithImages);
            }
        }

        /// <summary>
        /// Implementa API Composition Pattern: materializa resultados y carga imágenes con UNA sola llamada a la API
        /// </summary>
        private async Task<List<Home>> ComposeWithImagesAsync(List<Home> homes)
        {
            if (homes.Count == 0)
                return homes;

            // Extraer todas las ViviendaIds únicas
            var viviendaIds = homes
                .Where(x => !string.IsNullOrEmpty(x.ViviendaId))
                .Select(x => x.ViviendaId)
                .Distinct()
                .ToList();

            if (viviendaIds.Count == 0)
                return homes;

            // UNA SOLA petición con todas las IDs (concurrencia limitada a 8)
            var allImagesDictionary = new Dictionary<string, Core.Dto.HomeImagesDto>();
            try
            {
                var semaphore = new SemaphoreSlim(8);
                var tasks = viviendaIds.Select(async viviendaId =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var imagesDto = await _imageService.GetHomeImagesByViviendaIdAsync(viviendaId);
                        if (imagesDto != null)
                        {
                            lock (allImagesDictionary)
                            {
                                allImagesDictionary[viviendaId] = imagesDto;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching images for {viviendaId}: {ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in API Composition: {ex.Message}");
            }

            // Composición: asignar las imágenes a cada objeto
            foreach (var home in homes)
            {
                if (!string.IsNullOrEmpty(home.ViviendaId) && allImagesDictionary.TryGetValue(home.ViviendaId, out var imagesDto))
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

            return homes;
        }
    }
}