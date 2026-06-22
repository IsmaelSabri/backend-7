using System.Security.Cryptography;
using System.Text;
using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.impl
{
    public sealed class OtpService : IOtpService
    {
        private readonly CoreDb _db;

        public OtpService(CoreDb db)
        {
            _db = db;
        }

        public string Generate(int length = 6)
        {
            const string digits = "0123456789";

            var otp = new char[length];

            for (int i = 0; i < length; i++)
            {
                var randomByte = RandomNumberGenerator.GetInt32(digits.Length);

                otp[i] = digits[randomByte];
            }

            return new string(otp);
        }

        public string HashOtp(string otp)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(otp);

            var hash = sha.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }

        public async Task<PasswordResetOtp?> SaveOtpAsync(Guid userId, string code, DateTime expiresAt)
        {
            var previousOtps = await _db.PasswordResetOtps
                .Where(o => o.UserId == userId && !o.Used)
                .ToListAsync();

            foreach (var previousOtp in previousOtps)
            {
                previousOtp.Used = true;
            }

            var hashedCode = HashOtp(code);
            var otp = new PasswordResetOtp
            {
                UserId = userId,
                Code = hashedCode,
                ExpiresAt = expiresAt,
                Used = false,
                Attempts = 0
            };

            _db.PasswordResetOtps.Add(otp);
            await _db.SaveChangesAsync();
            return otp;
        }

        public async Task<PasswordResetOtp?> GetByUserIdAsync(Guid userId)
        {
            return await _db.PasswordResetOtps
                .Where(o => o.UserId == userId && !o.Used && o.ExpiresAt > DateTime.UtcNow && o.Attempts < 3)
                .OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<PasswordResetOtp?> ValidateOtpAsync(Guid userId, string code)
        {
            var otp = await GetByUserIdAsync(userId);
            if (otp == null)
                return null;

            var hashedInput = HashOtp(code);
            if (otp.Code != hashedInput)
            {
                if (otp.Attempts < 3)
                {
                    await IncrementAttemptsAsync(otp.Id);
                }
                return null;
            }

            return otp;
        }

        public async Task<bool> MarkOtpAsUsedAsync(int id)
        {
            var otp = await _db.PasswordResetOtps.FindAsync(id);
            if (otp == null)
                return false;

            otp.Used = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncrementAttemptsAsync(int id)
        {
            var otp = await _db.PasswordResetOtps.FindAsync(id);
            if (otp == null)
                return false;

            otp.Attempts++;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
