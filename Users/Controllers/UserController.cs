using Microsoft.AspNetCore.Mvc;
using Users.Dto;
using Users.Jwt;
using Users.Models;
using Users.Repositories;
using Users.Enums;
using Users.Services;
using AutoMapper;
using Users.Data;
using Sieve.Models;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserCollection db;
        private readonly JwtResource jwtResource = new();
        private readonly IPasswordHasher passwordHasher;
        private readonly IMapper mapper;
        private User user1;
        private readonly SieveProcessor sieveProcessor;

        public UserController(IMapper mapper1, IPasswordHasher passwordHasher1, UserDb hdb, SieveProcessor _sieveProcessor)
        {
            mapper = mapper1;
            passwordHasher = passwordHasher1;
            user1 = new();
            db = new UserCollection(hdb);
            sieveProcessor = _sieveProcessor;
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
                    + "Se ha enviado un correo a " + user.Email + " con más información.");
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
                user1.IsPro = false;
                user1.IsnotLocked = false;
                await db.NewUser(user1);
                db.SendWelcomeEmail(user1);
            }
            return Created("Created", true);
        }

        [HttpPut("full")]
        public async Task<IActionResult> CompleteRegistry([FromBody] UserReadyDto user)
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
                user1.ProfileImageAsString = "{\"imageId\":\"abcde\",\"imageName\":\"kjhjg-jpg\",\"imageUrl\":\"../../assets/img/blank_image.jpg\",\"imageDeleteUrl\":\"https://ibb.co/3kKyhNN/cec17dd74c1a240e64d9fb772bf23fc7\"}";
                var dump = ObjectDumper.Dump(user1);
                Console.WriteLine(dump);
                await db.UpdateUser(user1);
            }
            return Created("Ahora inicia sesion", true);
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] UserReadyDto user)
        {
            if (user == null)
            {
                return BadRequest("No existe una cuenta para ");
            }
            else
            {
                user1 = mapper.Map<User>(user);
                db.SendResetEmail(user1);
                return Ok();
            }
        }

        [HttpPost("save-newpassword")]
        public async Task<IActionResult> SaveNewPassword([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null !!");
            }
            else
            {
                user1 = mapper.Map<User>(user);
                if (!string.IsNullOrEmpty(user.Password))
                {
                    user1.Password = passwordHasher.Hash(user.Password);
                }
                user1.Isactive = true;
                user1.IsnotLocked = true;
                if (!string.IsNullOrEmpty(user1.Id))
                {
                    await db.UpdateUser(user1);
                }
                return Ok(user1);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            User user = new();
            try
            {
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    user = await db.GetUserByEmail(dto.Email);
                }
                if (!string.IsNullOrEmpty(user.Password) && !string.IsNullOrEmpty(dto.Password))
                {
                    // Incorrect password
                    if (!passwordHasher.Verify(user.Password, dto.Password))   // (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                    {
                        return BadRequest("Incorrect password. Please try again.");
                    }
                    /*
                    * El usuario ya existe en la bdd y lo ha introducido el password ok (caso ideal)
                    */
                    else if (passwordHasher.Verify(user.Password, dto.Password))
                    {
                        user.Token = jwtResource.Generate(user);
                        user.RefreshToken = jwtResource.CreateRefreshToken();
                        user.RefreshTokenDateExpires = DateTime.Now.ToLocalTime().AddDays(7);
                        user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                        if (!string.IsNullOrEmpty(user.Id))
                            await db.UpdateUser(user);
                    }
                }
            }
            catch (Exception)
            {
                // User does not exist
                return BadRequest("Email not found. Join us in least than 1 minute");
            }
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
            if (!string.IsNullOrEmpty(user.Id))
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
            if (user.Username?.Length == 0)
            {
                ModelState.AddModelError("Username", "Nombre de usuario no encontrado");
            }
            user.Id = id;
            await db.UpdateUser(user);
            return Created("Modified", user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await db.GetUserById(id);
            await db.DeleteUser(user);
            return Ok("Deleted successfully");
        }

        [HttpGet("query")] // return a collection
        public IActionResult GetQuery([FromQuery] SieveModel model)
        {
            var userResult = sieveProcessor.Apply(model, db.GetPagedUsers().AsNoTracking());
            return Ok(userResult);
        }

        [HttpGet("single-user")] // return single json if exists
        public IActionResult GetSingleQuery([FromQuery] SieveModel model)
        {
            var userResult = sieveProcessor.Apply(model, db.GetPagedUsers().AsNoTracking()).Single();
            return Ok(userResult);
        }
    }
}