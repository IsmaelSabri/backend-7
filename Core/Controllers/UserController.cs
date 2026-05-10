using Microsoft.AspNetCore.Mvc;
using Core.Dto;
using Core.Jwt;
using Core.Models;
using Core.Collections.impl;
using Core.Collections;
using Core.Enums;
using Core.Services;
using AutoMapper;
using Core.Data;
using Sieve.Models;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;
using Core.Extensions;
using Core.Collections.Impl;

namespace Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserCollection db;
        private readonly ICustomFilters CustomFilters;
        private readonly JwtResource jwtResource = new();
        private readonly IPasswordHasher passwordHasher;
        private readonly IMapper mapper;
        private User user1;
        private readonly SieveProcessor sieveProcessor;
        private readonly IImageService imageService;

        public UserController(IMapper mapper1, IPasswordHasher passwordHasher1, CoreDb hdb, SieveProcessor _sieveProcessor, IImageService imageService)
        {
            mapper = mapper1;
            passwordHasher = passwordHasher1;
            user1 = new();
            db = new UserCollection(hdb, imageService);
            CustomFilters = new CustomFiltersCollection();
            sieveProcessor = _sieveProcessor;
            this.imageService = imageService;
        }

        //[Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await db.GetAllUsers();
            await users.ComposeImagesAsync(imageService);
            return Ok(users);
        }

        private async Task<User?> ComposeUserImages(User? user)
        {
            if (user == null) return null;
            await user.ComposeImagesAsync(imageService);
            return user;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserDetails(Guid id)
        {
            var user = await ComposeUserImages(await db.GetUserById(id));
            return Ok(user);
        }

        [HttpGet("check/{id}")]
        public async Task<IActionResult> CheckUserBeforeRegister(string id)
        {
            var user = await ComposeUserImages(await db.GetUserByUserId(id));
            return Ok(user);
        }

        [HttpGet("checkemail/{email}")]
        public async Task<IActionResult> CheckUserByEmail(string email)
        {
            var user = await ComposeUserImages(await db.GetUserByEmail(email));
            return Ok(user);
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Email es requerido.");
            }

            var existingUser = await db.GetUserByEmail(user.Email);
            if (existingUser != null)
            {
                return BadRequest($"Ya existe una cuenta para {user.Email}. Recupera tu contraseña.");
            }

            user1 = mapper.Map<User>(user);
            user1.Username = $"{user.Firstname} {user.Lastname}".Trim();
            user1.DateRegistry = DateTime.UtcNow;
            user1.Role = nameof(Role.USER);
            user1.Isactive = false;
            user1.NormalizeNullStrings();
            await db.NewUser(user1);
            Utilities.SendWelcomeEmail(user1);
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
                user1.NormalizeNullStrings();
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
                Utilities.SendResetEmail(user1);
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
                if (user1.Id.IsValid())
                {
                    await db.UpdateUser(user1);
                }
                await user1.ComposeImagesAsync(imageService);
                return Ok(user1);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Email y contraseña son obligatorios.");
            }

            var user = await db.GetUserByEmail(dto.Email);
            if (user == null)
            {
                return BadRequest("No existe ningún usuario con ese email.");
            }

            if (!passwordHasher.Verify(user.Password, dto.Password))
            {
                return BadRequest("Contraseña incorrecta.");
            }

            user.Token = jwtResource.Generate(user);
            user.RefreshToken = jwtResource.CreateRefreshToken();
            user.RefreshTokenDateExpires = DateTime.UtcNow.AddDays(7);
            user.LastaccessDate = DateTime.UtcNow;
            if (user.Id.IsValid())
            {
                await db.UpdateUser(user);
            }

            await ComposeUserImages(user);
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
            if (user.Id.IsValid())
                await db.UpdateUser(user);
            await user.ComposeImagesAsync(imageService);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] User user, Guid id)
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
            var dump = ObjectDumper.Dump(user);
            Console.WriteLine(dump);
            await db.UpdateUser(user);
            await user.ComposeImagesAsync(imageService);
            return Created("Modified", user);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await db.GetUserById(id);
            if (user is { })
            {
                await db.DeleteUser(user);
                var response = new CustomHttpResponseDto
                {
                    HttpStatusCode = 200,
                    HttpStatus = "OK",
                    Reason = "Success",
                    Message = "Usuario eliminado exitosamente"
                };
                return Ok(response);
            }
            else
            {
                var response = new CustomHttpResponseDto
                {
                    HttpStatusCode = 400,
                    HttpStatus = "BAD_REQUEST",
                    Reason = "User not found",
                    Message = "El usuario no existe."
                };
                return BadRequest(response);
            }
        }

        [HttpGet("query")] // return a collection
        public async Task<IActionResult> GetQuery([FromQuery] SieveModel model)
        {
            var queryable = db.GetPagedUsers().AsNoTracking();
            var userResult = sieveProcessor.Apply(model, queryable);
            var userList = userResult.ToList();
            await userList.ComposeImagesAsync(imageService);
            return Ok(userList);
        }

        [HttpGet("single-user")] // return single json if exists
        public async Task<IActionResult> GetSingleQuery([FromQuery] SieveModel model)
        {
            var queryable = db.GetPagedUsers().AsNoTracking();
            var userResult = sieveProcessor.Apply(model, queryable).SingleOrDefault();
            if (userResult == null) return NotFound();
            await userResult.ComposeImagesAsync(imageService);
            return Ok(userResult);
        }
    }
}