using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Jwt;
using WebApi.Models;
using WebApi.Repositories;
using WebApi.Enums;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserCollection db = new UserCollection();
        private readonly JwtResource jwtResource = new();

        //[Authorize]
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
                    DateRegistry = DateTime.UtcNow,
                    Role = nameof(Role.USER)
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
            user.Token = jwtResource.Generate(user);
            user.RefreshToken = jwtResource.CreateRefreshToken();
            user.RefreshTokenDateExpires = DateTime.Now.AddDays(7);
            await db.UpdateUser(user);
            /*Response.Cookies.Append("token", jwt, new CookieOptions
            {
                HttpOnly = tru¡,
                IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Domain = "localhost", 
                Expires = DateTime.UtcNow.AddDays(14)
            });
            Response.Headers.Add("token", jwt);*/
            return Ok(user);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
        {
            if (tokenDto is null)
                return BadRequest("Invalid Client Request");
            string accessToken = tokenDto.AccessToken;
            string refreshToken = tokenDto.RefreshToken;
            var principal = jwtResource.GetPrincipleFromExpiredToken(accessToken);
            var username = principal.Identity.Name;//?.FindFirst(x => x.Type.Equals("Username"))?.Value;
            var user = await db.GetUserByUsername(username!);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenDateExpires <= DateTime.Now)
                return BadRequest("Invalid Request");
            user.RefreshToken = jwtResource.CreateRefreshToken();
            user.Token = jwtResource.Generate(user);
            await db.UpdateUser(user);
            return Ok(user);
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