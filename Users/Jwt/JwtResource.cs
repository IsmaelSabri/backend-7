using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Users.Models;
using System.Security.Cryptography;

namespace Users.Jwt
{
    public class JwtResource
    {
        private readonly string secureKey = "gcuq@Q3rnc38BH7ARAv5tqZruUUuvFerCf+kzZ48^6s(AF9PyUNy^s";
        //private readonly IUserCollection db = new UserCollection();

        public string Generate(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secureKey);
            ClaimsIdentity identity = new();
            if (!string.IsNullOrEmpty(user.Role) && !string.IsNullOrEmpty(user.Email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            }
            SigningCredentials credentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }


        public string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);
            /*var tokenInUser = db.GetUserByRefreshToken(refreshToken);
            if (tokenInUser!=null)
            {
                return CreateRefreshToken();
            }*/
            return refreshToken;
        }

        public ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("gcuq@Q3rnc38BH7ARAv5tqZruUUuvFerCf+kzZ48^6s(AF9PyUNy^s");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken?.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) != true)
                throw new SecurityTokenException("Invalid Token = Invalid session");
            return principal;
        }
    }
}