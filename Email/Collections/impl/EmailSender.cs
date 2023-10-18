using Email.Model;
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
        public EmailSender(IOptions<EmailConfiguration> options)
        {
            emailConfiguration = options.Value;
        }
        public async Task SendEmailAsync(EmailDto emailDto, string action)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(emailConfiguration.FromAddress)
            };
            email.To.Add(MailboxAddress.Parse(emailDto.ToEmail));
            email.Subject = emailDto.Subject;
            switch (action)
            {
                case "Welcome!":
                    if (emailDto.Name != null && emailDto.Message != null)
                    {
                        var builder = new BodyBuilder
                        {
                            HtmlBody = WelcomeHTML(emailDto.Name, emailDto.Message)
                        };
                        email.Body = builder.ToMessageBody();
                    }
                    break;
                case "Reset":
                    if (emailDto.Name != null && emailDto.Message != null)
                    {
                        var builder = new BodyBuilder
                        {
                            HtmlBody = SetPasswordHTML(emailDto.Name, emailDto.Message)
                        };
                        email.Body = builder.ToMessageBody();
                    }
                    break;
            }
            using var smtp = new SmtpClient();
            smtp.Connect(emailConfiguration.ServerAddress, emailConfiguration.ServerPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailConfiguration.FromAddress, emailConfiguration.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        
        public string WelcomeHTML(string name, string message)
        {
            var html = File.ReadAllText(@"./Assets/Welcome.html");
            html = html.Replace("{{name}}", name);
            html = html.Replace("{{message}}", message);
            return html;
        }

        public string SetPasswordHTML(string name, string message)
        {
            var html = File.ReadAllText(@"./Assets/SetPassword.html");
            html = html.Replace("{{name}}", name);
            html = html.Replace("{{message}}", message);
            return html;
        }
    }
}