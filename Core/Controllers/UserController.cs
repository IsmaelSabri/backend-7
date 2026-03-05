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

namespace Core.Controllers
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
        private readonly IImageService imageService;

        public UserController(IMapper mapper1, IPasswordHasher passwordHasher1, CoreDb hdb, SieveProcessor _sieveProcessor, IImageService imageService)
        {
            mapper = mapper1;
            passwordHasher = passwordHasher1;
            user1 = new();
            db = new UserCollection(hdb, imageService);
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserDetails(Guid id)
        {
            var user = await db.GetUserById(id);
            if (user != null)
            {
                await user.ComposeImagesAsync(imageService);
            }
            return Ok(user);
        }

        [HttpGet("check/{id}")]
        public async Task<IActionResult> CheckUserBeforeRegister(string id)
        {
            var user = await db.GetUserByUserId(id);
            if (user != null)
            {
                await user.ComposeImagesAsync(imageService);
            }
            return Ok(user);
        }

        [HttpGet("checkemail/{email}")]
        public async Task<IActionResult> CheckUserByEmail(string email)
        {
            var user = await db.GetUserByEmail(email);
            if (user != null)
            {
                await user.ComposeImagesAsync(imageService);
            }
            return Ok(user);
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            var dump = ObjectDumper.Dump(user);
            Console.WriteLine(dump);
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
                user1.Username = user.Firstname + " " + user.Lastname;
                user1.DateRegistry = DateTime.UtcNow.ToLocalTime();
                user1.Role = nameof(Role.USER);
                user1.Isactive = false;
                await db.NewUser(user1);
                Utilities.SendWelcomeEmail(user1);
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
            User user = new();
            try
            {
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var user1 = await db.GetUserByEmail(dto.Email);
                    if (user1 is { })
                    {
                        user = user1;
                    }
                    else
                    {
                        return BadRequest("No existe ningún usuario con el email " + dto.Email + " !!");
                    }
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
                        if (user.Id.IsValid())
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
            await user.ComposeImagesAsync(imageService);
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
            
            // Convertir a lista y componer imágenes
            var userList = userResult.ToList();
            await userList.ComposeImagesAsync(imageService);
            
            return Ok(userList);
        }

        [HttpGet("single-user")] // return single json if exists
        public async Task<IActionResult> GetSingleQuery([FromQuery] SieveModel model)
        {
            var queryable = db.GetPagedUsers().AsNoTracking();
            var userResult = sieveProcessor.Apply(model, queryable).Single();
            
            // Componer imágenes en el usuario individual
            await userResult.ComposeImagesAsync(imageService);
            
            return Ok(userResult);
        }
    }
}