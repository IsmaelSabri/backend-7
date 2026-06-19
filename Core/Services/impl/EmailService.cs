using System.Net.Http;
using System.Net.Http.Json;
using Core.Services;
using Microsoft.Extensions.Logging;

namespace Core.Services.impl
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(HttpClient httpClient, ILogger<EmailService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string name, string email, string verificationToken)
        {
            var payload = new { Name = name, ToEmail = email, Subject = "Activar cuenta", Token = verificationToken };
            await SendEmailAsync("/api/email/setpassword", payload);
        }

        public async Task SendResetEmailAsync(string name, string email, string userId)
        {
            var payload = new { Name = name, ToEmail = email, Subject = "Cambiar contraseña", Message = userId };
            await SendEmailAsync("/api/email/resendpassword", payload);
        }

        public async Task SendOtpEmailAsync(string name, string email, string otp)
        {
            var payload = new { Name = name, ToEmail = email, Subject = "Tu código de verificación", Message = otp };
            await SendEmailAsync("/api/email/send-otp", payload);
        }

        private async Task SendEmailAsync(string url, object payload)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Email no enviado. URL: {Url}, Status: {Status}", url, response.StatusCode);
                }
                else
                {
                    _logger.LogInformation("Email enviado correctamente. URL: {Url}", url);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email a {Payload}", payload);
                throw;
            }
        }
    }
}
