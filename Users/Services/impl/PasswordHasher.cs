using System.Resources;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Users.Services.impl
{
    public class PasswordHasher : IPasswordHasher
    {
        public const int SaltByteSize = 16;
        public const int HashByteSize = 32;
        public const int i = 10000;
        private static readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA256;

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltByteSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, i, algorithm, HashByteSize);
            return string.Join(';', Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public bool Verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(';');
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);
            var input = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, i, algorithm, HashByteSize);
            return CryptographicOperations.FixedTimeEquals(hash, input);
        }


    }
}