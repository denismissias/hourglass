using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Hourglass.Request;
using Hourglass.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hourglass.Controllers
{
    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UserController(IUserRepository userRepository) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "plain/text")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized, "plain/text")]
        public async Task<ActionResult<UserResponse>> Create([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await userRepository.ExistsAsync(request.Login))
                return BadRequest($"The User {request.Login} already exists.");

            var user = new User(request);

            return CreatedAtAction(null, new UserResponse(await userRepository.CreateAsync(user)));
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "plain/text")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized, "plain/text")]
        public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await userRepository.ExistsAsync(id))
                return NotFound($"User Id {id} not found.");

            var user = new User(request)
            {
                Id = id
            };

            return Ok(new UserResponse(await userRepository.UpdateAsync(user)));
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound, "plain/text")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized, "plain/text")]
        public async Task<ActionResult<UserResponse>> Get(int id)
        {
            var user = await userRepository.GetAsync(id);

            if (user == null)
                return NotFound($"User Id {id} not found.");

            return Ok(new UserResponse(user));
        }
    }
}
