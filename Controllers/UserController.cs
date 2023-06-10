using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Jwt;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserCollection db = new UserCollection();
        private readonly JwtResource jwtResource = new();

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await db.GetAllUsers());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            return Ok(await db.GetUserById(id));
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (user.Username == null)
            {
                ModelState.AddModelError("Username", "Nombre de usuario no encontrado");
            }
            else
            {
                var newUser = new User
                {
                    Username = user.Username,
                    Email = user.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    DateRegistry = DateTime.UtcNow
                };
                await db.NewUser(newUser);
            }
            return Created("Created", true);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            User user = new();
            try
            {
                user = await db.GetUserByEmail(dto.Email);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Email not found. Join us in least than 1 minute" });
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest(new { message = "Incorrect password. Please try again." });
            }
            var jwt = jwtResource.Generate(user.Id);
            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });

            return Ok(new
            {
                message = "success"
            });
        }

         [HttpGet("user")]
        public IActionResult Auth()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = jwtResource.Verify(jwt!);

                var user = db.GetUserById(token.Issuer);

                return Ok(user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return Ok(new
            {
                message = "success"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] User user, string id)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (user.Username == string.Empty)
            {
                ModelState.AddModelError("Username", "Nombre de usuario no encontrado");
            }
            user.Id = new MongoDB.Bson.ObjectId(id);
            await db.UpdateUser(user);
            return Created("Modified", true);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await db.DeleteUser(id);
            return Ok("Deleted successfully"); // success
        }
    }
}