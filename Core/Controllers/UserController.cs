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
using Core.Extensions;
using Sieve.Models;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;
using Core.Collections.Impl;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string RefreshTokenCookieName = "fc_refresh_token";
        private readonly IUserCollection db;
        private readonly ICustomFilters CustomFilters;
        private readonly JwtResource jwtResource = new();
        private readonly IPasswordHasher passwordHasher;
        private readonly IMapper mapper;
        private readonly IAuthService authService;
        private readonly IOtpService otpService;
        private readonly IEmailService emailService;
        private readonly ITokenService tokenService;
        private readonly CoreDb coreDb;
        private User user1;
        private readonly SieveProcessor sieveProcessor;
        private readonly IImageService imageService;
        private readonly ILogger<UserController> _logger;

        public UserController(IMapper mapper1, IPasswordHasher passwordHasher1, IAuthService authService1, IOtpService otpService1, IEmailService emailService1, ITokenService tokenService1, CoreDb hdb, SieveProcessor _sieveProcessor, IImageService imageService, ILogger<UserController> logger)
        {
            _logger = logger;
            mapper = mapper1;
            passwordHasher = passwordHasher1;
            authService = authService1;
            otpService = otpService1;
            emailService = emailService1;
            tokenService = tokenService1;
            coreDb = hdb;
            user1 = new();
            db = new UserCollection(hdb, imageService);
            CustomFilters = new CustomFiltersCollection();
            sieveProcessor = _sieveProcessor;
            this.imageService = imageService;
        }

        private static string HashRefreshToken(string refreshToken)
        {
            return Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(refreshToken)
                )
            );
        }

        private string? GetClientIpAddress()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(realIp))
            {
                return realIp;
            }

            return Request.HttpContext.Connection.RemoteIpAddress?.ToString();
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
            if (user == null)
                return null;
            await user.ComposeImagesAsync(imageService);
            return user;
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                IsEssential = true
            });
        }

        private void ClearRefreshTokenCookie()
        {
            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Email is required.");
            }

            var existingUser = await db.GetUserByEmail(user.Email);

            if (existingUser != null)
            {
                return BadRequest(
                    "Ya existe una cuenta asociada a este correo."
                );
            }

            var newUser = mapper.Map<User>(user);

            newUser.UserId = Guid.NewGuid().ToString();

            newUser.Username =
                $"{user.Firstname} {user.Lastname}".Trim();

            newUser.DateRegistry = DateTime.UtcNow;

            newUser.Role = nameof(Role.USER);

            newUser.Isactive = false;

            newUser.NormalizeNullStrings();

            await db.NewUser(newUser);

            // GENERAR TOKEN DE VERIFICACIÓN

            var verificationToken =
                Convert.ToHexString(
                    RandomNumberGenerator.GetBytes(32)
                );

            var verificationTokenHash =
                Convert.ToHexString(
                    SHA256.HashData(
                        System.Text.Encoding.UTF8.GetBytes(verificationToken)
                    )
                );

            await tokenService.CreateAsync(
                newUser.UserId!,
                verificationTokenHash,
                DateTime.UtcNow.AddHours(24)
            );

            // ENVIAR EMAIL

            await emailService.SendWelcomeEmailAsync(
                newUser.Username ?? "Usuario",
                newUser.Email ?? string.Empty,
                verificationToken
            );

            return Ok(new
            {
                message =
                    "Cuenta creada. Verifica tu correo electrónico."
            });
        }

        [HttpPut("complete-registration")]
        public async Task<IActionResult> CompleteRegistry(
            [FromBody] RegistryDto registryDto)
        {
            if (registryDto == null)
            {
                return BadRequest("Datos inválidos.");
            }

            if (string.IsNullOrWhiteSpace(registryDto.Token))
            {
                return BadRequest("Enlace inválido.");
            }

            if (string.IsNullOrWhiteSpace(registryDto.Password))
            {
                return BadRequest("La contraseña es obligatoria.");
            }

            // VALIDACIÓN BACKEND PASSWORD

            var passwordRegex =
                new Regex(
                    @"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*#?&^_-]).{8,}"
                );

            if (!passwordRegex.IsMatch(registryDto.Password))
            {
                return BadRequest(
                    "La contraseña no cumple los requisitos."
                );
            }

            var tokenHash = Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(registryDto.Token)
                )
            );

            var verificationToken =
                await tokenService.GetByTokenHashAsync(tokenHash);

            if (
                verificationToken == null ||
                verificationToken.Used ||
                verificationToken.ExpiresAt <= DateTime.UtcNow
            )
            {
                return BadRequest(
                    "El enlace no es válido o ha expirado."
                );
            }

            var user =
                await db.GetUserByUserId(
                    verificationToken.UserId
                );

            if (user == null)
            {
                return BadRequest(
                    "El enlace no es válido o ha expirado."
                );
            }

            user.Password =
                passwordHasher.Hash(registryDto.Password);

            user.Isactive = true;

            user.NormalizeNullStrings();

            verificationToken.Used = true;

            await db.UpdateUser(user);

            await tokenService.UpdateAsync(verificationToken);

            return Ok(new
            {
                message = "Cuenta activada correctamente."
            });
        }


        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(
            [FromBody] PasswordResetRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new { message = "El email es obligatorio." });
            }

            await authService.RequestPasswordReset(dto.Email);

            return Ok(new
            {
                message =
                  "Revisa tu correo."
            });
        }

        [HttpPost("validate-password-reset-otp")]
        public async Task<IActionResult> ValidatePasswordResetOtp(
            [FromBody] ValidatePasswordResetOtpDto dto)
        {
            if (
                dto == null ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Otp)
            )
            {
                return BadRequest(new { message = "El email y el OTP son obligatorios." });
            }

            var resetToken = await authService.ValidatePasswordResetOtp(dto.Email, dto.Otp);

            if (string.IsNullOrWhiteSpace(resetToken))
            {
                return BadRequest(new
                {
                    message = "Código OTP inválido, caducado o ya utilizado."
                });
            }

            return Ok(new
            {
                message = "Código verificado correctamente.",
                resetToken,
            });
        }

        [HttpPost("reset-password-with-token")]
        public async Task<IActionResult> ResetPasswordWithToken(
            [FromBody] ResetPasswordWithTokenDto dto)
        {
            if (
                dto == null ||
                string.IsNullOrWhiteSpace(dto.Token) ||
                string.IsNullOrWhiteSpace(dto.Password)
            )
            {
                return BadRequest(new { message = "Token y contraseña son obligatorios." });
            }

            var updated = await authService.ResetPasswordWithToken(dto.Token, dto.Password);

            if (!updated)
            {
                return BadRequest(new
                {
                    message = "No se pudo cambiar la contraseña. El token no es válido o ha expirado."
                });
            }

            return Ok(new
            {
                message = "Contraseña cambiada correctamente."
            });
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
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Credenciales incorrectas.");
            }

            var user = await db.GetUserByEmail(dto.Email);

            var fakeHash =
                "4tTQWUk9Qm5LQnR4VGJ4Zw==;Y7x8pS6Pj9g9Q0wM1t7JxQ==";

            if (user == null)
            {
                passwordHasher.Verify(fakeHash, dto.Password);

                return BadRequest("Credenciales incorrectas.");
            }

            if (string.IsNullOrWhiteSpace(user.Password) ||
                !passwordHasher.Verify(user.Password, dto.Password))
            {
                return BadRequest("Credenciales incorrectas.");
            }

            if (!user.Isactive)
            {
                return BadRequest(
                    "Debes verificar tu correo electrónico."
                );
            }

            var accessToken = jwtResource.Generate(user);
            var refreshToken = jwtResource.CreateRefreshToken();
            var refreshTokenHash = HashRefreshToken(refreshToken);
            var clientIpAddress = !string.IsNullOrWhiteSpace(dto.IpAddress)
                ? dto.IpAddress
                : GetClientIpAddress();

            user.LastaccessDate = DateTime.UtcNow;

            if (user.Id.IsValid())
            {
                await db.UpdateUser(user);
            }

            var expiresAt = DateTime.UtcNow.AddDays(7);

            var userRefreshSession = new UserRefreshSession
            {
                UserId = user.UserId ?? user.Id.ToString(),
                SessionId = Guid.NewGuid().ToString(),
                TokenHash = refreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                LastSeenAt = DateTime.UtcNow,
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IpAddress = clientIpAddress,
                RevokedAt = null
            };

            await coreDb.UserRefreshSessions.AddAsync(userRefreshSession);
            await coreDb.SaveChangesAsync();

            await ComposeUserImages(user);

            SetRefreshTokenCookie(refreshToken);

            return Ok(new
            {
                accessToken,
                user = new
                {
                    user.Id,
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.Role,
                    user.ProfileImage
                }
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var refreshTokenHash = HashRefreshToken(refreshToken);

                var session = await coreDb.UserRefreshSessions
                    .FirstOrDefaultAsync(x =>
                        x.TokenHash == refreshTokenHash &&
                        x.RevokedAt == null);

                if (session != null)
                {
                    session.RevokedAt = DateTime.UtcNow;
                    await coreDb.SaveChangesAsync();
                }
            }

            ClearRefreshTokenCookie();

            return Ok(new { message = "Sesión cerrada correctamente." });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest("Invalid request.");
            }

            var refreshTokenHash = HashRefreshToken(refreshToken);

            var userRefreshSession = await coreDb.UserRefreshSessions
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == refreshTokenHash &&
                    x.RevokedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow);

            if (userRefreshSession == null)
            {
                return BadRequest("Invalid request.");
            }

            var user = await db.GetUserByUserId(userRefreshSession.UserId);

            if (user == null)
            {
                return BadRequest("Invalid request.");
            }

            var newAccessToken = jwtResource.Generate(user);
            var newRefreshToken = jwtResource.CreateRefreshToken();
            var newRefreshTokenHash = HashRefreshToken(newRefreshToken);

            user.LastaccessDate = DateTime.UtcNow;

            await db.UpdateUser(user);

            var nextExpiresAt = DateTime.UtcNow.AddDays(7);

            userRefreshSession.TokenHash = newRefreshTokenHash;
            userRefreshSession.ExpiresAt = nextExpiresAt;
            userRefreshSession.LastSeenAt = DateTime.UtcNow;
            userRefreshSession.UserAgent = Request.Headers["User-Agent"].ToString();
            userRefreshSession.IpAddress ??= GetClientIpAddress();

            coreDb.UserRefreshSessions.Update(userRefreshSession);
            await coreDb.SaveChangesAsync();

            SetRefreshTokenCookie(newRefreshToken);

            await user.ComposeImagesAsync(imageService);

            return Ok(new
            {
                accessToken = newAccessToken,
            });
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
            if (userResult == null)
                return NotFound();
            await userResult.ComposeImagesAsync(imageService);
            return Ok(userResult);
        }
    }
}