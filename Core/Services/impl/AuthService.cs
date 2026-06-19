using Core.Collections;
using Core.Services;

namespace Core.Services.impl
{
    public class AuthService : IAuthService
    {
        private readonly IOtpService otpService;
        private readonly IUserCollection userCollection;
        private readonly IEmailService emailService;

        public AuthService(IOtpService otpService1, IUserCollection userCollection1, IEmailService emailService1)
        {
            otpService = otpService1;
            userCollection = userCollection1;
            emailService = emailService1;
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
            await emailService.SendOtpEmailAsync(user.Username, email, otp);
        }
    }
}
