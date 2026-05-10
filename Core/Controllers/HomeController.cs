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
        private readonly SieveProcessor sieveProcessor;
        private readonly IImageService imageService;

        public HomeController(IMapper mapper1, CoreDb hdb, SieveProcessor _sieveProcessor, IImageService imageService)
        {
            mapper = mapper1;
            db = new HomeCollection(hdb, imageService);
            sieveProcessor = _sieveProcessor;
            this.imageService = imageService;
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

        [HttpGet("imgbb-health")]
        public async Task<IActionResult> CheckImgbb()
        {
            try
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(3);

                var res = await http.GetAsync("https://api.imgbb.com/");

                return Ok(new { ok = res.IsSuccessStatusCode });
            }
            catch
            {
                return Ok(new { ok = false });
            }
        }

        [HttpPost("dummy-upload")]
        public IActionResult DummyUpload()
        {
            return Ok();
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateHome([FromBody] HomeDto homeDto)
        {
            if (homeDto == null)
            {
                return BadRequest("Home data is required.");
            }

            var home = MapHomeDto(homeDto);
            if (home == null)
            {
                return BadRequest("Modelo de propiedad no válido.");
            }

            if (!TryParseCoordinates(homeDto.Lat, homeDto.Lng, out var lat, out var lng))
            {
                return BadRequest("Coordenadas no válidas.");
            }

            var priceProvider = new NumberFormatInfo { NumberDecimalSeparator = "," };
            home.Lat = lat;
            home.Lng = lng;
            var precioInicial = ParseNullableInt(homeDto.PrecioInicial, priceProvider);
            var precioFinal = ParseNullableInt(homeDto.PrecioFinal, priceProvider);
            var precioAlquiler = ParseNullableInt(homeDto.PrecioAlquiler, priceProvider);
            if (precioInicial.HasValue)
                home.PrecioInicial = precioInicial.Value;
            if (precioFinal.HasValue)
                home.PrecioFinal = precioFinal.Value;
            if (precioAlquiler.HasValue)
                home.PrecioAlquiler = precioAlquiler.Value;
            home.FechaCreacion = DateTime.UtcNow;
            home.FechaUltimaModificacion = DateTime.UtcNow;
            home.NormalizeNullStrings();

            await db.NewHome(home);
            return Created("Created", home);
        }

        private Home? MapHomeDto(HomeDto homeDto)
        {
            switch (homeDto.Model)
            {
                case "Flat":
                    return mapper.Map<Flat>(homeDto);
                case "House":
                    return mapper.Map<House>(homeDto);
                case "Room":
                    return mapper.Map<Room>(homeDto);
                case "HolidayRent":
                    return mapper.Map<HolidayRent>(homeDto);
                case "NewProject":
                    return mapper.Map<NewProject>(homeDto);
                case "Other":
                {
                    var other = mapper.Map<Other>(homeDto);
                    var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };
                    var superficieGarage = ParseNullableFloat(homeDto.SuperficieGarage, provider);
                    var superficieTrastero = ParseNullableFloat(homeDto.SuperficieTrastero, provider);
                    var alturaTrastero = ParseNullableFloat(homeDto.AlturaTrastero, provider);
                    if (superficieGarage.HasValue)
                        other.SuperficieGarage = superficieGarage.Value;
                    if (superficieTrastero.HasValue)
                        other.SuperficieTrastero = superficieTrastero.Value;
                    if (alturaTrastero.HasValue)
                        other.AlturaTrastero = alturaTrastero.Value;
                    return other;
                }
                default:
                    return null;
            }
        }

        private static int? ParseNullableInt(string value, NumberFormatInfo provider)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return int.TryParse(value, NumberStyles.Any, provider, out var parsed) ? parsed : null;
        }

        private static float? ParseNullableFloat(string value, NumberFormatInfo provider)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return float.TryParse(value, NumberStyles.Any, provider, out var parsed) ? parsed : null;
        }

        private static bool TryParseCoordinates(string? latValue, string? lngValue, out double lat, out double lng)
        {
            lat = 0;
            lng = 0;
            if (string.IsNullOrWhiteSpace(latValue) || string.IsNullOrWhiteSpace(lngValue))
                return false;
            var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            return double.TryParse(latValue, NumberStyles.Any, provider, out lat)
                && double.TryParse(lngValue, NumberStyles.Any, provider, out lng);
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

        [HttpGet("query")]
        public async Task<IActionResult> GetQuery([FromQuery] SieveModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Filters) && model.Filters.Contains("lng"))
            {
                if (!TryParseBoundingBox(model.Filters, out var points, out var collectionModel, out var normalizedFilter))
                {
                    return BadRequest("Filtros de coordenadas inválidos.");
                }

                model.Filters = normalizedFilter;
                return collectionModel switch
                {
                    "Flat" => Ok(sieveProcessor.Apply(model, db.GetBoxedFlats(points[0], points[2], points[1], points[3]).AsNoTracking()).ToList()),
                    "House" => Ok(sieveProcessor.Apply(model, db.GetBoxedHouses(points[0], points[2], points[1], points[3]).AsNoTracking()).ToList()),
                    "Room" => Ok(sieveProcessor.Apply(model, db.GetBoxedRooms(points[0], points[2], points[1], points[3]).AsNoTracking()).ToList()),
                    "HolidayRent" => Ok(sieveProcessor.Apply(model, db.GetBoxedHolidayRent(points[0], points[2], points[1], points[3]).AsNoTracking()).ToList()),
                    "NewProject" => Ok(sieveProcessor.Apply(model, db.GetBoxedNewProjects(points[0], points[2], points[1], points[3]).AsNoTracking()).ToList()),
                    "Other" => Ok(sieveProcessor.Apply(model, db.GetBoxedOthers(points[0], points[2], points[1], points[3]).AsNoTracking()).ToList()),
                    _ => Ok(sieveProcessor.Apply(model, db.GetBoxedHomes(points[0], points[2], points[1], points[3]).AsNoTracking()).ToList()),
                };
            }

            if (!string.IsNullOrWhiteSpace(model.Filters) && model.Filters.Contains("model@=*"))
            {
                var collectionModel = ExtractModelFromFilter(model.Filters);
                return collectionModel switch
                {
                    "Flat" => Ok(sieveProcessor.Apply(model, db.GetPagedFlats().AsNoTracking()).ToList()),
                    "House" => Ok(sieveProcessor.Apply(model, db.GetPagedHouses().AsNoTracking()).ToList()),
                    "Room" => Ok(sieveProcessor.Apply(model, db.GetPagedRooms().AsNoTracking()).ToList()),
                    "HolidayRent" => Ok(sieveProcessor.Apply(model, db.GetPagedHolidayRent().AsNoTracking()).ToList()),
                    "NewProject" => Ok(sieveProcessor.Apply(model, db.GetPagedNewProjects().AsNoTracking()).ToList()),
                    "Other" => Ok(sieveProcessor.Apply(model, db.GetPagedOthers().AsNoTracking()).ToList()),
                    _ => Ok(sieveProcessor.Apply(model, db.GetPagedHomes().AsNoTracking()).ToList()),
                };
            }

            var normalQuery = sieveProcessor.Apply(model, db.GetPagedHomes().AsNoTracking()).ToList();
            return Ok(normalQuery);
        }

        private static string ExtractModelFromFilter(string filters)
        {
            var terms = filters.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                if (term.StartsWith("model@=*"))
                {
                    return term[8..];
                }
            }
            return string.Empty;
        }

        private bool TryParseBoundingBox(string filters, out double[] pointsOut, out string collectionModel, out string normalizedFilters)
        {
            pointsOut = new double[4];
            collectionModel = string.Empty;
            normalizedFilters = string.Empty;

            var provider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            var subs = filters.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var keep = new List<string>();
            foreach (var sub in subs)
            {
                if (sub.StartsWith("lng>="))
                {
                    pointsOut[0] = double.TryParse(sub[5..], NumberStyles.Any, provider, out var value) ? value : 0;
                }
                else if (sub.StartsWith("lng<="))
                {
                    pointsOut[1] = double.TryParse(sub[5..], NumberStyles.Any, provider, out var value) ? value : 0;
                }
                else if (sub.StartsWith("lat>="))
                {
                    pointsOut[2] = double.TryParse(sub[5..], NumberStyles.Any, provider, out var value) ? value : 0;
                }
                else if (sub.StartsWith("lat<="))
                {
                    pointsOut[3] = double.TryParse(sub[5..], NumberStyles.Any, provider, out var value) ? value : 0;
                }
                else if (sub.StartsWith("model@=*"))
                {
                    collectionModel = sub[8..];
                }
                else
                {
                    keep.Add(sub);
                }
            }

            normalizedFilters = string.Join(',', keep);
            return true;
        }

        /// <summary>
        /// Cosultas sobre el arrays !!!
        /// </summary>
        [HttpGet("by-like-me-forever/{userId}")]
        public async Task<IActionResult> GetHomesByLikeMeForever(string userId)
        {
            return Ok(await db.GetHomesByLikeMeForever(userId));
        }
        [HttpGet("discard-me/{userId}")]
        public async Task<IActionResult> GetDiscardedHomes(string userId)
        {
            return Ok(await db.GetDiscardedHomes(userId));
        }


    }
}