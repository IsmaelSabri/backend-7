using Microsoft.AspNetCore.Mvc;
using Users.Dto;
using Users.Jwt;
using Users.Models;
using Users.Enums;
using Users.Services;
using AutoMapper;
using Sieve.Services;
using Sieve.Models;
using Microsoft.EntityFrameworkCore;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IElasticService<User> elasticService, IMapper mapper1, IPasswordHasher passwordHasher1,
    SieveProcessor _sieveProcessor) : ControllerBase
    {
        private readonly JwtResource jwtResource = new();
        private readonly IPasswordHasher passwordHasher = passwordHasher1;
        private readonly IMapper mapper = mapper1;
        private User user1 = new();
        private readonly IElasticService<User> _elasticService = elasticService;
        private readonly SieveProcessor sieveProcessor = _sieveProcessor;

        //[Authorize]
        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            return Ok(_elasticService.GetAllPagedDocuments());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            var document = await _elasticService.GetDocumentAsync(long.Parse(id));
            //Console.WriteLine(homeDto);
            if (document == null)
            {
                return NotFound("User not found");
            }
            return Ok(document);
        }

        [HttpGet("check/{id}")]
        public async Task<IActionResult> CheckUserBeforeRegister(string id)
        {
            var document = await _elasticService.GetDocumentByUserIdAsync(id);
            return Ok(document);
        }

        [HttpGet("checkemail/{email}")]
        public async Task<IActionResult> CheckUserByEmail(string email)
        {
            var document = await _elasticService.GetDocumentByEmailAsync(email);
            return Ok(document);
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
                var userExists = await _elasticService.GetDocumentByEmailAsync(user.Email);
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
                //Guid myuuid = Guid.NewGuid();
                //user1.Id = myuuid.ToString().Replace("-","");
                user1.UserId = _elasticService.GenerateRandomAlphanumericString();
                user1.Id = _elasticService.GenerateRandomAlphanumericString();
                user1.Username = user.Firstname + " " + user.Lastname;
                user1.DateRegistry = DateTime.UtcNow.ToLocalTime();
                user1.Role = nameof(Role.USER);
                user1.Isactive = false;
                user1.IsPro = false;
                user1.IsnotLocked = false;
                var result = await _elasticService.AddDocumentAsync(user1);
                _elasticService.SendWelcomeEmail(user1);
                return Ok(user1);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("full/{id}")]
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
                user1.ProfileImageAsString = "{\"imageId\":\"abcde\",\"imageName\":\"kjhjg-jpg\",\"imageUrl\":\"../../assets/img/blank_image.jpg\",\"imageDeleteUrl\":\"https://ibb.co/3kKyhNN/cec17dd74c1a240e64d9fb772bf23fc7\"}";
                var dump = ObjectDumper.Dump(user1);
                Console.WriteLine(dump);
                string document = await _elasticService.UpdateDocumentAsync(user1);
                return Created(document, user1);
            }
            else
            {
                return BadRequest("Something wrong updating " + user.Firstname);
            }
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
                _elasticService.SendResetEmail(user1);
                return Ok();
            }
        }

        [HttpPost("save-newpassword")]
        public async Task<IActionResult> SaveNewPassword([FromBody] UserReadyDto user)
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
                    var result = await _elasticService.UpdateDocumentAsync(user1);
                    return Ok(user1);
                }
                else
                {
                    return BadRequest("Error saving password");
                }

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
                    user = await _elasticService.GetDocumentByEmailAsync(dto.Email);
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
                        Console.WriteLine("dentro");
                        user.Token = jwtResource.Generate(user);
                        user.RefreshToken = jwtResource.CreateRefreshToken();
                        user.RefreshTokenDateExpires = DateTime.Now.ToLocalTime().AddDays(7);
                        user.LastaccessDate = DateTime.UtcNow.ToLocalTime();
                        if (!string.IsNullOrEmpty(user.Id))
                        {
                            await _elasticService.UpdateDocumentAsync(user);
                        }
                        return Ok(user);
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
            return BadRequest();
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
            var user = await _elasticService.GetDocumentByUsernameAsync(username!);
            if (user is null)
                return BadRequest("Invalid Request. Cannot find user.");
            if (user.RefreshToken != refreshToken)
                return BadRequest("Invalid Request. Invalid token.");
            if (user.RefreshTokenDateExpires <= DateTime.Now.ToLocalTime())
                return BadRequest("Invalid Request. Token expired.");
            user.RefreshToken = jwtResource.CreateRefreshToken();
            user.Token = jwtResource.Generate(user);
            if (!string.IsNullOrEmpty(user.Id))
                await _elasticService.UpdateDocumentAsync(user);
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
            string result = await _elasticService.UpdateDocumentAsync(user);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            Console.WriteLine(long.Parse(id));
            var home = await _elasticService.DeleteDocumentAsync(long.Parse(id));
            return Ok(home);
        }

        [HttpGet("query")] // return a collection
        public IActionResult GetQuery([FromQuery] SieveModel model)
        {
            var userResult = sieveProcessor.Apply(model, _elasticService.GetAllPagedDocuments().AsNoTracking());
            return Ok(userResult);
        }

        [HttpGet("check-user")] // return single json if exists
        public IActionResult GetSingleQuery([FromQuery] SieveModel model)
        {
            var userResult = sieveProcessor.Apply(model, _elasticService.GetAllPagedDocuments().AsNoTracking()).Single();
            return Ok(userResult);
        }
    }
}