using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Hourglass.Request;
using Hourglass.Response;
using Hourglass.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hourglass.Controllers
{
    /// <summary>
    /// Authentication controller for user login and token generation
    /// </summary>
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AuthenticationController(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<AuthenticationController> logger) : ControllerBase
    {
        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="request">The authentication request with login and password</param>
        /// <returns>Authentication response with JWT token and user info</returns>
        [HttpPost]
        [Route("authenticate")]
        [ProducesResponseType(typeof(AuthenticationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "plain/text")]
        public async Task<ActionResult<AuthenticationResponse>> Authenticate([FromBody] AuthenticationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userRepository.GetAsync(request.Login);

            if (!PasswordIsValid(request, user))
            {
                logger.LogWarning("Failed authentication attempt for login: {Login}", request.Login);
                return BadRequest("Invalid Credentials");
            }

            // user is guaranteed to be non-null at this point due to PasswordIsValid check
            var token = tokenService.Generate(user!);

            logger.LogInformation("User authenticated successfully: {UserId}", user!.Id);

            return new AuthenticationResponse
            {
                Token = token,
                User = new UserResponse(user!)
            };
        }

        private static bool PasswordIsValid(AuthenticationRequest request, User? user)
        {
            return user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        }
    }
}
