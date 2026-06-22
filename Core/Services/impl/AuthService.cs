using Core.Collections;
using Core.Services;
using Core.Extensions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Services.impl
{
    public class AuthService : IAuthService
    {
        private readonly IOtpService otpService;
        private readonly IResetTokenService resetTokenService;
        private readonly IUserCollection userCollection;
        private readonly IEmailService emailService;
        private readonly IPasswordHasher passwordHasher;

        public AuthService(
            IOtpService otpService1,
            IResetTokenService resetTokenService1,
            IUserCollection userCollection1,
            IEmailService emailService1,
            IPasswordHasher passwordHasher1)
        {
            otpService = otpService1;
            resetTokenService = resetTokenService1;
            userCollection = userCollection1;
            emailService = emailService1;
            passwordHasher = passwordHasher1;
        }

        public async Task RequestPasswordReset(string email)
        {
            var otp = otpService.Generate();

            var user = await userCollection.GetUserByEmail(email);
            if (user == null)
            {
                Console.WriteLine($"No user found with email: {email}");
                return;
            }
            await otpService.SaveOtpAsync(
                user.Id,
                otp,
                DateTime.UtcNow.AddMinutes(10)
            );
            await emailService.SendOtpEmailAsync(user.Username ?? "Usuario", email, otp);
        }

        public async Task<string?> ValidatePasswordResetOtp(string email, string otp)
        {
            var user = await userCollection.GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }

            var validOtp = await otpService.ValidateOtpAsync(user.Id, otp);
            if (validOtp == null)
            {
                return null;
            }

            await otpService.MarkOtpAsUsedAsync(validOtp.Id);

            var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var resetTokenHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(resetToken))
            );

            await resetTokenService.CreateAsync(
                user.Id,
                resetTokenHash,
                DateTime.UtcNow.AddMinutes(10)
            );

            return resetToken;
        }

        public async Task<bool> ResetPasswordWithToken(string token, string password)
        {
            var passwordRegex =
                new Regex(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*#?&^_-]).{8,}");

            if (!passwordRegex.IsMatch(password))
            {
                return false;
            }

            var tokenHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(token))
            );

            var resetToken = await resetTokenService.GetByTokenHashAsync(tokenHash);

            if (resetToken == null || resetToken.Used || resetToken.ExpiresAt <= DateTime.UtcNow)
            {
                return false;
            }

            var user = await userCollection.GetUserById(resetToken.UserId);
            if (user == null)
            {
                return false;
            }

            user.Password = passwordHasher.Hash(password);
            user.Isactive = true;
            user.NormalizeNullStrings();
            await userCollection.UpdateUser(user);

            resetToken.Used = true;
            await resetTokenService.UpdateAsync(resetToken);

            return true;
        }
    }
}
