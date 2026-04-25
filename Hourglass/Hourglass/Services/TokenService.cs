using Hourglass.Configuration;
using Hourglass.Models;
using Hourglass.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hourglass.Services
{
    /// <summary>
    /// Service for generating JWT tokens
    /// </summary>
    public class TokenService(JwtSettings jwtSettings) : ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user to generate a token for</param>
        /// <returns>The generated JWT token string</returns>
        public string Generate(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new(ClaimTypes.Name, user.Name),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString())
                ]),
                Issuer = jwtSettings.Issuer,
                Expires = DateTime.UtcNow.AddHours(jwtSettings.ExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
