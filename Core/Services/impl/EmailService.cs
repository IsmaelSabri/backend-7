using System.Net.Http;
using System.Net.Http.Json;
using Core.Dto;
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
                _logger.LogInformation(
                    "Enviando email a URL: {Url} con payload: {Payload}",
                    url,
                    payload
                );

                var response = await _httpClient.PostAsJsonAsync(url, payload);

                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Email no enviado. URL: {Url}, Status: {Status}, Response: {Response}",
                        url,
                        response.StatusCode,
                        responseBody
                    );

                    return;
                }

                _logger.LogInformation(
                    "Email enviado correctamente. URL: {Url}, Response: {Response}",
                    url,
                    responseBody
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error enviando email a {Payload}",
                    payload
                );

                throw;
            }
        }
    }
}
