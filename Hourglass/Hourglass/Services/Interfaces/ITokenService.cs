using Hourglass.Models;

namespace Hourglass.Services.Interfaces
{
    /// <summary>
    /// Service for generating authentication tokens
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user to generate a token for</param>
        /// <returns>The generated JWT token string</returns>
        string Generate(User user);
    }
}
