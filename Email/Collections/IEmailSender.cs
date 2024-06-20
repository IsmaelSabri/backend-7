using Email.Dto;

namespace Email.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailDto emailDto, string action);
    }
}