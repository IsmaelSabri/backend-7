using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.Dto;
using Users.Jwt;
using Users.Models;
using Users.Repositories;
using Users.Enums;
using Email.Dto;
using MongoDB.Bson.IO;
using System.Text.Json;
using RestSharp;
using MongoDB.Bson;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserCollection db = new UserCollection();
        private readonly JwtResource jwtResource = new();
        private readonly HttpClient Client = new();

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

        [HttpGet("check/{id}")]
        public async Task<IActionResult> CheckUserBeforeRegister(string id)
        {
            return Ok(await db.GetUserByUserId(id));
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            if (user.Email == null)
            {
                return BadRequest();
            }
            try{
            var userExists = await db.GetUserByEmail(user.Email);
            Console.WriteLine(userExists);
            if (userExists != null)
            {
                return BadRequest("Ya existe una cuenta para " + user.Email + "\n\nPuede iniciar sesión si recupera su contraseña.\n\n"
                +"Se ha enviado un correo a " + user.Email + " con mas información.");

            }
            }catch(Exception){
                
             }
                var newUser = new User
                {
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    UserId = db.GenerateRandomAlphanumericString(),
                    Username = "",
                    DateRegistry = DateTime.UtcNow.ToLocalTime(),
                    Role = nameof(Role.USER),
                    Isactive = false,
                    IsnotLocked = false,
                };
                await db.NewUser(newUser);
                db.SendWelcomeEmail(newUser);
                return Created("Created", true);
        }

        [HttpPut("fullregistry")]
        public async Task<IActionResult> CompleteRegistry(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            var newUser = new User
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Firstname + " " + user.Lastname,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Email = user.Email,
                UserId = user.UserId,
                DateRegistry = user.DateRegistry,
                LastaccessDate = DateTime.UtcNow.ToLocalTime(),
                Role = user.Role,
                Isactive = true,
                IsnotLocked = true,
                fotoPerfilUrl = "https://robohash.org/" + user.Firstname
                + user.Lastname + db.GenerateRandomAlphanumericString().Substring(3, 8),
            };
            await db.UpdateUser(newUser);
            return Created("Modified", true);
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
            user.RefreshTokenDateExpires = DateTime.Now.ToLocalTime().AddDays(7);
            user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
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
            var username = principal.Identity?.Name;//?.FindFirst(x => x.Type.Equals("Username"))?.Value;
            var user = await db.GetUserByUsername(username!);
            if (user is null)
                return BadRequest("Invalid Request. Cannot find user.");
            if (user.RefreshToken != refreshToken)
                return BadRequest("Invalid Request. Invalid token.");
            if (user.RefreshTokenDateExpires <= DateTime.Now.ToLocalTime())
                return BadRequest("Invalid Request. Token expired.");
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
            await db.UpdateUser(user);
            return Created("Modified", true);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = db.GetUserByUserId(id);
            await db.DeleteUser(id);
            return Ok("Deleted successfully"); // success
        }
    }
}