namespace Core.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string name, string email, string userId);
        Task SendResetEmailAsync(string name, string email, string userId);
        Task SendOtpEmailAsync(string name, string email, string otp);
    }
}
