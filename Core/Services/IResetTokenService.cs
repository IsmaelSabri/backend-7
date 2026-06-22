using Core.Models;

namespace Core.Services
{
    public interface IResetTokenService
    {
        Task<PasswordResetToken> CreateAsync(Guid userId, string tokenHash, DateTime expiresAt);
        Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash);
        Task<bool> UpdateAsync(PasswordResetToken token);
    }
}