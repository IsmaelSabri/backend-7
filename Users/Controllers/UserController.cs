using Microsoft.AspNetCore.Mvc;
using Users.Dto;
using Users.Jwt;
using Users.Models;
using Users.Repositories;
using Users.Enums;
using Users.Services;
using AutoMapper;
using System;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserCollection db = new UserCollection();
        private readonly JwtResource jwtResource = new();
        private readonly IPasswordHasher passwordHasher;
        private readonly IMapper mapper;
        private User user1;
        public UserController(IMapper mapper1, IPasswordHasher passwordHasher1)
        {
            mapper = mapper1;
            passwordHasher = passwordHasher1;
            user1 = new();
        }

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

        [HttpGet("checkemail/{email}")]
        public async Task<IActionResult> CheckUserByEmail(string email)
        {
            return Ok(await db.GetUserByEmail(email));
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (user.Email == null)
            {
                ModelState.AddModelError("Email", "Debe incluir una dirección de correo válida.");
                return BadRequest("Email not found");
            }
            try
            {
                var userExists = await db.GetUserByEmail(user.Email);
                Console.WriteLine(userExists);
                if (userExists != null)
                {
                    return BadRequest("Ya existe una cuenta para " + user.Email + "\n\nPuede iniciar sesión si recupera su contraseña.\n\n"
                    + "Se ha enviado un correo a " + user.Email + " con mas información.");
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }
            if (ModelState.IsValid)
            {
                user1 = mapper.Map<User>(user);
                user1.UserId = db.GenerateRandomAlphanumericString();
                user1.Username = user.Firstname + " " + user.Lastname;
                user1.DateRegistry = DateTime.UtcNow.ToLocalTime();
                user1.Role = nameof(Role.USER);
                user1.Isactive = false;
                user1.IsnotLocked = false;
                await db.NewUser(user1);
                db.SendWelcomeEmail(user1);
            }
            return Created("Created", true);
        }

        [HttpPut("fullregistry/{id}")]
        public async Task<IActionResult> CompleteRegistry([FromBody] UserReadyDto user, string id)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (user.Password == null)
            {
                ModelState.AddModelError("Password", "La contraseña no puede estar vacía.");
                return BadRequest("Missing password");
            }
            if (ModelState.IsValid)
            {
                user1 = mapper.Map<User>(user);
                user1.Password = passwordHasher.Hash(user.Password);
                user1.Isactive = true;
                user1.IsnotLocked = true;
                var dump = ObjectDumper.Dump(user1);
                Console.WriteLine(dump);
                await db.UpdateUser(user1, id);
            }
            return Created("Ahora inicia sesión", true);
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
            if (user.Password != null && !passwordHasher.Verify(user.Password, dto.Password))   // (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest(new { message = "Incorrect password. Please try again." });
            }
            user.Token = jwtResource.Generate(user);
            user.RefreshToken = jwtResource.CreateRefreshToken();
            user.RefreshTokenDateExpires = DateTime.Now.ToLocalTime().AddDays(7);
            user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
            await db.UpdateUser(user, user.Id);
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
            await db.UpdateUser(user, user.Id);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] User user, string id)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (user.Username?.Length == 0)
            {
                ModelState.AddModelError("Username", "Nombre de usuario no encontrado");
            }
            user.Id = id;
            await db.UpdateUser(user, id);
            return Created("Modified", user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = db.GetUserById(id);
            await db.DeleteUser(id);
            return Ok("Deleted successfully");
        }
    }
}