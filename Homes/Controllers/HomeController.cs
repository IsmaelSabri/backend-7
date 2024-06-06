using Microsoft.AspNetCore.Mvc;

using Homes.Services;
using Homes.Models;
using Homes.Dto;
using AutoMapper;
using System.Globalization;
using Sieve.Services;
using Sieve.Models;
using Microsoft.EntityFrameworkCore;
namespace Homes.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IElasticService<Home> _elasticService;

        private readonly IMapper mapper;
        private Home home;
        private readonly SieveProcessor sieveProcessor;
        public HomeController(IMapper mapper1, IElasticService<Home> elasticService, SieveProcessor _sieveProcessor)
        {
            mapper = mapper1;
            _elasticService = elasticService;
            sieveProcessor = _sieveProcessor;
        }

        [HttpGet("All")]
        public IActionResult GetAllHomes()
        {
            return Ok(_elasticService.GetAllPagedDocuments());
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
                home.ViviendaId = _elasticService.GenerateRandomAlphanumericString();
                home.Id = _elasticService.GenerateRandomAlphanumericString();
                var dump = ObjectDumper.Dump(home);
                Console.WriteLine(dump);
                string res = await _elasticService.AddDocumentAsync(home);
                return Ok(home);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHomeDetails(string id)
        {
            var document = await _elasticService.GetDocumentAsync(id);
            //Console.WriteLine(homeDto);
            if (document == null)
            {
                return NotFound("Home not found");
            }
            return Ok(document);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDocument([FromBody] HomeDto homeDto)
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
            }
            string result = await _elasticService.UpdateDocumentAsync(home);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHome(string id)
        {
            string message = await _elasticService.DeleteDocumentAsync(id);
            return Ok(message);
        }

        [HttpGet("query")]
        public IActionResult GetQuery([FromQuery] SieveModel model)
        {
            var homeResult = sieveProcessor.Apply(model, _elasticService.GetAllPagedDocuments().AsNoTracking());
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
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchFlatDocumentsAsync().AsNoTracking());
                        break;
                    case "House":
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchHouseDocumentsAsync().AsNoTracking());
                        break;
                    case "Room":
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchRoomDocumentsAsync().AsNoTracking());
                        break;
                    case "HolidayRent":
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchHolidayRentDocumentsAsync().AsNoTracking());
                        break;
                    case "NewProject":
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchNewProjectDocumentsAsync().AsNoTracking());
                        break;
                }
            }
            return Ok(homeResult);
        }

        [HttpGet("check-home")]
        public IActionResult GetSingleHomeByQuery([FromQuery] SieveModel model)
        {
            var homeResult = sieveProcessor.Apply(model, _elasticService.GetAllPagedDocuments().AsNoTracking()).Single();
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
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchFlatDocumentsAsync().AsNoTracking()).Single();
                        break;
                    case "House":
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchHouseDocumentsAsync().AsNoTracking()).Single();
                        break;
                    case "Room":
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchRoomDocumentsAsync().AsNoTracking()).Single();
                        break;
                    case "HolidayRent":
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchHolidayRentDocumentsAsync().AsNoTracking()).Single();
                        break;
                    case "NewProject":
                        homeResult = sieveProcessor.Apply(model, _elasticService.SearchNewProjectDocumentsAsync().AsNoTracking()).Single();
                        break;
                }
            }
            return Ok(homeResult);
        }
    }
}