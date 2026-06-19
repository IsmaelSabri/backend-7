using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.impl
{
    public sealed class TokenService : ITokenService
    {
        private readonly CoreDb _db;

        public TokenService(CoreDb db)
        {
            _db = db;
        }

        public async Task<EmailVerificationToken> CreateAsync(string userId, string tokenHash, DateTime expiresAt)
        {
            var verificationToken = new EmailVerificationToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
                Used = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.EmailVerificationTokens.Add(verificationToken);
            await _db.SaveChangesAsync();
            return verificationToken;
        }

        public async Task<EmailVerificationToken?> GetByIdAsync(int id)
        {
            return await _db.EmailVerificationTokens.FindAsync(id);
        }

        public async Task<EmailVerificationToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _db.EmailVerificationTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        }

        public async Task<EmailVerificationToken?> GetByUserIdAsync(string userId)
        {
            return await _db.EmailVerificationTokens
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(EmailVerificationToken token)
        {
            _db.EmailVerificationTokens.Update(token);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var token = await _db.EmailVerificationTokens.FindAsync(id);
            if (token == null)
            {
                return false;
            }

            _db.EmailVerificationTokens.Remove(token);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
