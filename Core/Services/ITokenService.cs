using Core.Models;

namespace Core.Services
{
    public interface ITokenService
    {
        Task<EmailVerificationToken> CreateAsync(string userId, string tokenHash, DateTime expiresAt);
        Task<EmailVerificationToken?> GetByIdAsync(int id);
        Task<EmailVerificationToken?> GetByTokenHashAsync(string tokenHash);
        Task<EmailVerificationToken?> GetByUserIdAsync(string userId);
        Task<bool> UpdateAsync(EmailVerificationToken token);
        Task<bool> DeleteAsync(int id);
    }
}
