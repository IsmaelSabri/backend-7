using Microsoft.AspNetCore.Mvc;

using Homes.Services;
using Homes.Models;
using Homes.Dto;
using AutoMapper;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
namespace Homes.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EsController : ControllerBase
    {
        private readonly IElasticService<Home> _elasticService;

        private readonly IMapper mapper;
        private Home home;
        public EsController(IMapper mapper1, IElasticService<Home> elasticService)
        {
            mapper = mapper1;
            _elasticService = elasticService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllHomes()
        {
            return Ok(await _elasticService.GetAllDocuments());
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
                var dump = ObjectDumper.Dump(home);
                Console.WriteLine(dump);
                var res = await _elasticService.AddDocumentAsync(home);
                return Created("Created", res);
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> GetHomeDetails(string id, [FromBody] HomeDto homeDto)
        {
            var document = await _elasticService.GetDocumentAsync(int.Parse(id));
            Console.WriteLine(homeDto);
            if (document==null)
            {
                return NotFound();
            }
            return Ok(document);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDocument([FromBody] HomeDto homeDto){
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
                }}
            var result = await _elasticService.UpdateDocumentAsync(home);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHome(int id)
        {
            var home = await _elasticService.DeleteDocumentAsync(id);
            return Ok("Deleted");
        }
    }
}