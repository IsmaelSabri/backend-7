using Email.Dto;
using Email.Models;

namespace Email.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailDto emailDto, string action);
        Task SendEmailAsync(ContactMessage message, string action);
        Task SendEmailAsync(UsersContactMessage message, string action);
    }
}