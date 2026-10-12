using Email.Data;
using Email.Dto;
using Email.Models;
using Email.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Email.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IEmailSender emailSender;
        private readonly EmailDbContext dbContext;

        public EmailController(ILogger<EmailController> logger, IEmailSender emailSender, EmailDbContext dbContext)
        {
            _logger = logger;
            this.emailSender = emailSender;
            this.dbContext = dbContext;
        }

        [HttpPost("setpassword")]
        public async Task<IActionResult> Index(EmailDto emailDto)
        {
            try
            {
                if (emailDto == null)
                {
                    return BadRequest();
                }
                else
                {
                    await emailSender.SendEmailAsync(emailDto, "Welcome!");
                    return Ok("Enviado !!");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("resendpassword")]
        public async Task<IActionResult> ReIndex(EmailDto emailDto)
        {
            try
            {
                if (emailDto == null)
                {
                    return BadRequest();
                }
                else
                {
                    await emailSender.SendEmailAsync(emailDto, "Reset");
                    return Ok("Enviado !!");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("email-contact")]
        public async Task<IActionResult> ContactEmailAsync(UsersContactMessage contactMessage)
        {
            try
            {
                if (contactMessage == null)
                {
                    return BadRequest();
                }

                await emailSender.SendEmailAsync(contactMessage, "MessageBetweenUsers");

                var contactMessageEntity = new UsersContactMessage
                {
                    UserId = contactMessage.UserId ?? string.Empty,
                    FromEmail = contactMessage.FromEmail ?? string.Empty,
                    ToEmail = contactMessage.ToEmail ?? string.Empty,
                    ToName = contactMessage.ToName ?? string.Empty,
                    PropertyLink = contactMessage.PropertyLink ?? "Url del anuncio no disponible",
                    PropertyThumbnail = contactMessage.PropertyThumbnail ?? "Miniatura no disponible",
                    Name = contactMessage.Name ?? string.Empty,
                    Company = contactMessage.Company ?? string.Empty,
                    Subject = contactMessage.Subject ?? "Nuevo interés por tu anuncio",
                    Message = contactMessage.Message ?? string.Empty,
                    Phone = contactMessage.Phone ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                };

                dbContext.UsersContactMessages.Add(contactMessageEntity);
                await dbContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Enviado correctamente."
                });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost("contact-message")]
        public async Task<IActionResult> ContactMessageAsync(ContactMessage message)
        {
            if (message == null)
            {
                return BadRequest();
            }

            message.Reference = string.IsNullOrWhiteSpace(message.Reference)
                ? $"FC-{DateTime.UtcNow:yyMMdd}{DateTime.UtcNow.Ticks % 10000:D4}"
                : message.Reference;

            message.CreatedAt = DateTime.UtcNow;
            message.Status = "pending";

            dbContext.ContactMessages.Add(message);
            await dbContext.SaveChangesAsync();

            var companyEmail = new ContactMessage
            {
                Name = message.Name,
                Email = "isma_sl@inbox.lv",
                UserId = message.UserId,
                Subject = $"[Nueva consulta #{message.Reference}] {message.Category}",
                Reference = message.Reference,
                Category = message.Category,
                Message = $"Usuario: {message.Name}\n\nEmail: {message.Email}\n\nMensaje: {message.Message}",
                Phone = message.Phone,
                Company = message.Company,
            };

            var customerEmail = new EmailDto
            {
                FromName = "FuturCasa",
                ToEmail = message.Email,
                Subject = message.Subject,
                Name = message.Name,
                Message = message.Message,
            };
            _logger.LogInformation($"Sending contact message emails for reference {customerEmail}");
            await emailSender.SendEmailAsync(companyEmail, "Contact");
            await emailSender.SendEmailAsync(customerEmail, "ContactConfirmation");

            return Ok(new { message = "Mensaje guardado y enviado." });
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtpAsync(EmailDto emailDto)
        {
            try
            {
                if (emailDto == null)
                {
                    return BadRequest();
                }
                else
                {
                    await emailSender.SendEmailAsync(emailDto, "OTP");
                    return Ok("Enviado !!");
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}