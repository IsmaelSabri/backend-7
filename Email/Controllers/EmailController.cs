using Email.Dto;
using Email.Service;
using Microsoft.AspNetCore.Mvc;

namespace Email.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IEmailSender emailSender;

        public EmailController(ILogger<EmailController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            this.emailSender = emailSender;
        }

        [HttpGet("setpassword")]
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

        [HttpGet("resendpassword")]
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
        public async Task<IActionResult> ContactEmailAsync(EmailDto emailDto)
        {
            try
            {
                if (emailDto == null)
                {
                    return BadRequest();
                }
                else
                {
                    await emailSender.SendEmailAsync(emailDto, "Contact");
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