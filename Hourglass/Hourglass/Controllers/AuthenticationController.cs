using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Hourglass.Request;
using Hourglass.Response;
using Hourglass.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hourglass.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AuthenticationController(IUserRepository userRepository, ITokenService tokenService) : ControllerBase
    {
        [HttpPost]
        [Route("authenticate")]
        [ProducesResponseType(typeof(AuthenticationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "plain/text")]
        public async Task<ActionResult<AuthenticationResponse>> Authenticate([FromBody] AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userRepository.GetAsync(request.Login);

            if (!PasswordIsValid(request, user))
                return BadRequest("Invalid Credentials");

            var token = tokenService.Generate(user);

            return new AuthenticationResponse
            {
                Token = token,
                User = new UserResponse(user)
            };
        }

        private static bool PasswordIsValid(AuthenticationRequest request, User user)
        {
            return BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        }
    }
}
