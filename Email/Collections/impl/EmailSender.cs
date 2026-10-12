using Email.Models;
using Microsoft.Extensions.Options;
using Email.Dto;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace Email.Service.impl
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration emailConfiguration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailConfiguration> options, ILogger<EmailSender> logger)
        {
            emailConfiguration = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailDto emailDto, string action)
        {
            if (emailDto == null)
            {
                throw new ArgumentNullException(nameof(emailDto));
            }

            var email = BuildMessage(emailDto.ToEmail, emailDto.CcEmail, emailDto.Subject ?? "FuturCasa", "");

            switch (action)
            {
                case "Welcome!":
                    if (!string.IsNullOrWhiteSpace(emailDto.Name) && !string.IsNullOrWhiteSpace(emailDto.Token))
                    {
                        email.Body = new BodyBuilder { HtmlBody = WelcomeHTML(emailDto.Name, emailDto.Token) }.ToMessageBody();
                    }
                    break;

                case "Reset":
                    if (!string.IsNullOrWhiteSpace(emailDto.Name) && !string.IsNullOrWhiteSpace(emailDto.Message))
                    {
                        email.Body = new BodyBuilder { HtmlBody = SetPasswordHTML(emailDto.Name, emailDto.Message) }.ToMessageBody();
                    }
                    break;

                case "Contact":
                    email.Body = new BodyBuilder { HtmlBody = ContactUser(emailDto) }.ToMessageBody();
                    break;

                case "ContactConfirmation":
                    if (!string.IsNullOrWhiteSpace(emailDto.Name) && !string.IsNullOrWhiteSpace(emailDto.Subject))
                    {
                        email.Body = new BodyBuilder { HtmlBody = ContactConfirmationHTML(emailDto.Name, emailDto.Subject) }.ToMessageBody();
                    }
                    break;

                case "OTP":
                    if (!string.IsNullOrWhiteSpace(emailDto.Name) && !string.IsNullOrWhiteSpace(emailDto.Message))
                    {
                        email.Body = new BodyBuilder { HtmlBody = OtpHTML(emailDto.Name, emailDto.Message) }.ToMessageBody();
                    }
                    break;

                case "MessageBetweenUsers":
                    email.Body = new BodyBuilder { HtmlBody = MessageBetweenUsersHTML(emailDto) }.ToMessageBody();
                    break;
            }

            await SendAsync(email);
        }

        public Task SendEmailAsync(ContactMessage message, string action)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (action == "Contact")
            {
                var email = BuildMessage(message.Email, null, $"[Nueva consulta #{message.Reference}] {message.Category}", string.Empty);
                email.Body = new BodyBuilder { HtmlBody = ContactCompanyHTML(message) }.ToMessageBody();
                return SendAsync(email);
            }

            throw new NotSupportedException($"La acción '{action}' no está soportada para ContactMessage.");
        }

        public Task SendEmailAsync(UsersContactMessage message, string action)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (action == "MessageBetweenUsers")
            {
                var email = BuildMessage(message.ToEmail, null, message.Subject ?? "Nuevo interés por tu anuncio", string.Empty);
                email.Body = new BodyBuilder { HtmlBody = MessageBetweenUsersHTML(message) }.ToMessageBody();
                return SendAsync(email);
            }

            throw new NotSupportedException($"La acción '{action}' no está soportada para UsersContactMessage.");
        }

        public string WelcomeHTML(string name, string token)
        {
            var html = File.ReadAllText(@"./Assets/Welcome.html");
            html = html.Replace("{{name}}", name);
            html = html.Replace("{{token}}", token);
            return html;
        }

        public string SetPasswordHTML(string name, string token)
        {
            var html = File.ReadAllText(@"./Assets/SetPassword.html");
            html = html.Replace("{{name}}", name);
            html = html.Replace("{{token}}", token);
            return html;
        }

        public string ContactUser(EmailDto emailDto)
        {
            var html = File.ReadAllText(@"./Assets/ContactCompany.html");
            html = html.Replace("{{reference}}", "");
            html = html.Replace("{{category}}", "Consulta");
            html = html.Replace("{{subject}}", emailDto.Subject ?? "Sin asunto");
            html = html.Replace("{{fromName}}", emailDto.FromName ?? "Sin nombre");
            html = html.Replace("{{fromAddress}}", emailDto.FromAddress ?? "Sin email");
            html = html.Replace("{{phone}}", emailDto.Phone ?? "");
            html = html.Replace("{{message}}", emailDto.Message ?? "");
            return html;
        }

        public string ContactCompanyHTML(ContactMessage message)
        {
            var html = File.ReadAllText(@"./Assets/ContactCompany.html");
            html = html.Replace("{{reference}}", message.Reference ?? "Sin referencia");
            html = html.Replace("{{category}}", message.Category ?? "Consulta");
            html = html.Replace("{{subject}}", message.Subject ?? "Sin asunto");
            html = html.Replace("{{fromName}}", message.Name ?? "Sin nombre");
            html = html.Replace("{{fromAddress}}", message.Email ?? "Sin email");
            html = html.Replace("{{userId}}", message.UserId ?? "Usuario no registrado");
            html = html.Replace("{{phone}}", message.Phone ?? "");
            html = html.Replace("{{message}}", message.Message ?? "");
            return html;
        }

        private static bool IsValidEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            return MailboxAddress.TryParse(value, out _);
        }

        public string ContactConfirmationHTML(string name, string subject)
        {
            var html = File.ReadAllText(@"./Assets/ContactConfirmation.html");
            html = html.Replace("{{name}}", name);
            html = html.Replace("{{subject}}", subject);
            return html;
        }

        public string MessageBetweenUsersHTML(EmailDto emailDto)
        {
            var html = File.ReadAllText(@"./Assets/MessageBetweenUsers.html");
            html = html.Replace("{{name}}", emailDto.Name ?? "Interesado");
            html = html.Replace("{{toName}}", emailDto.ToName ?? "Propietario");
            html = html.Replace("{{subject}}", emailDto.Subject ?? "Nuevo interés por tu anuncio");
            html = html.Replace("{{fromName}}", emailDto.FromName ?? "Sin nombre");
            html = html.Replace("{{fromAddress}}", emailDto.FromAddress ?? "Sin email");
            html = html.Replace("{{phone}}", emailDto.Phone ?? "No indicado");
            html = html.Replace("{{message}}", emailDto.Message ?? "Sin mensaje");
            html = html.Replace("{{propertyLink}}", "#");
            html = html.Replace("{{thumbnail}}", "https://via.placeholder.com/150x130?text=Propiedad");
            html = html.Replace("{{category}}", "Propiedad");
            return html;
        }

        public string MessageBetweenUsersHTML(UsersContactMessage message)
        {
            var html = File.ReadAllText(@"./Assets/MessageBetweenUsers.html");
            html = html.Replace("{{name}}", message.Name ?? "Interesado");
            html = html.Replace("{{toName}}", message.ToName ?? "Propietario");
            html = html.Replace("{{subject}}", message.Subject ?? "Nuevo interés por tu anuncio");
            html = html.Replace("{{fromName}}", message.Name ?? "Sin nombre");
            html = html.Replace("{{fromAddress}}", message.FromEmail ?? "Sin email");
            html = html.Replace("{{phone}}", message.Phone ?? "No indicado");
            html = html.Replace("{{message}}", message.Message ?? "Sin mensaje");
            html = html.Replace("{{propertyLink}}", message.PropertyLink ?? "#");
            html = html.Replace("{{thumbnail}}", message.PropertyThumbnail ?? "https://via.placeholder.com/150x130?text=Propiedad");
            html = html.Replace("{{category}}", "Propiedad");
            return html;
        }

        public string OtpHTML(string name, string code)
        {
            var html = File.ReadAllText(@"./Assets/SendOtp.html");
            html = html.Replace("{{name}}", name);
            html = html.Replace("{{code}}", code);
            return html;
        }

        private MimeMessage BuildMessage(string? toEmail, string? ccEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(emailConfiguration.FromAddress))
            {
                throw new InvalidOperationException("La dirección del remitente no está configurada.");
            }

            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(emailConfiguration.FromAddress),
                Subject = subject ?? "FuturCasa"
            };

            if (!string.IsNullOrWhiteSpace(toEmail))
            {
                email.To.Add(MailboxAddress.Parse(toEmail));
            }

            if (!string.IsNullOrWhiteSpace(ccEmail) && IsValidEmail(ccEmail))
            {
                _logger.LogInformation("Adding CC email: {CcEmail}", ccEmail);
                email.Cc.Add(MailboxAddress.Parse(ccEmail));
            }

            email.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();
            return email;
        }

        public async Task SendAsync(MimeMessage email)
        {
            if (!email.To.Any() && !email.Cc.Any() && !email.Bcc.Any())
            {
                throw new InvalidOperationException(
                    "No se puede enviar el email porque no se ha especificado ningún destinatario.");
            }

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                emailConfiguration.ServerAddress,
                emailConfiguration.ServerPort,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                emailConfiguration.FromAddress,
                emailConfiguration.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}