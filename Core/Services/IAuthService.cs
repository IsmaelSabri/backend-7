using Core.Models;

namespace Core.Services
{
    public interface IAuthService
    {
        Task RequestPasswordReset(string email);
        Task<string?> ValidatePasswordResetOtp(string email, string otp);
        Task<bool> ResetPasswordWithToken(string token, string password);
    }
}
