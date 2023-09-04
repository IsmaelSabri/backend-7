using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Homes.Collections;
using Homes.Data;
using Homes.Dto;
using Homes.Models;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Homes.Infrastructure;

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
        public HomeController(IMapper mapper1, IOptions<CloudinarySettings> config, HouseDb hdb)
        {
            mapper = mapper1;
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            cloudinary = new Cloudinary(account);
            db = new HomeCollection(hdb);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllHomes()
        {
            return Ok(await db.GetAllHomes());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHomeDetails(string id)
        {
            return Ok(await db.GetHomeById(Convert.ToInt32(id)));
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
                    default:
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
                /*cloudinary*/
                if (homeDto.Foto != null)
                {
                    await using var stream = homeDto.Foto.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(homeDto.Foto.FileName, stream),
                        Transformation = new Transformation().Crop("fill")
                    };
                    var Result = await cloudinary.UploadAsync(uploadParams);
                    if (Result.Error != null)
                    {
                        throw new Exception(Result.Error.Message);
                    }
                    home.ImageName = Result.OriginalFilename;
                    home.ImageUrl = Result.Url.ToString();
                    home.ImageId = Result.PublicId;
                }
                //var dump= ObjectDumper.Dump(home);
                //Console.WriteLine(dump);
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
    }
}