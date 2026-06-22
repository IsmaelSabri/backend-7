using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.impl
{
    public sealed class ResetTokenService : IResetTokenService
    {
        private readonly CoreDb db;

        public ResetTokenService(CoreDb db)
        {
            this.db = db;
        }

        public async Task<PasswordResetToken> CreateAsync(Guid userId, string tokenHash, DateTime expiresAt)
        {
            var previousActiveTokens = await db.PasswordResetTokens
                .Where(x => x.UserId == userId && !x.Used)
                .ToListAsync();

            foreach (var activeToken in previousActiveTokens)
            {
                activeToken.Used = true;
            }

            var resetToken = new PasswordResetToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
                Used = false,
                CreatedAt = DateTime.UtcNow,
            };

            db.PasswordResetTokens.Add(resetToken);
            await db.SaveChangesAsync();
            return resetToken;
        }

        public async Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await db.PasswordResetTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        }

        public async Task<bool> UpdateAsync(PasswordResetToken token)
        {
            db.PasswordResetTokens.Update(token);
            return await db.SaveChangesAsync() > 0;
        }
    }
}