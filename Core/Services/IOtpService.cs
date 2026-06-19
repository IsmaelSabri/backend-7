using Core.Models;

namespace Core.Services
{
    public interface IOtpService
    {
        string Generate(int length = 6);
        string HashOtp(string otp);
        Task<PasswordResetOtp?> SaveOtpAsync(Guid userId, string code, DateTime expiresAt);
        Task<PasswordResetOtp?> GetByUserIdAsync(Guid userId);
        Task<PasswordResetOtp?> ValidateOtpAsync(Guid userId, string code);
        Task<bool> MarkOtpAsUsedAsync(int id);
        Task<bool> IncrementAttemptsAsync(int id);
    }
}
