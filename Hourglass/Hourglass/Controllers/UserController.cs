using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Hourglass.Request;
using Hourglass.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hourglass.Controllers
{
    /// <summary>
    /// User management controller
    /// </summary>
    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UserController(IUserRepository userRepository, ILogger<UserController> logger) : ControllerBase
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="request">The user creation request</param>
        /// <returns>The created user response</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "plain/text")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized, "plain/text")]
        public async Task<ActionResult<UserResponse>> Create([FromBody] UserRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await userRepository.ExistsAsync(request.Login))
            {
                logger.LogWarning("Attempted to create duplicate user with login: {Login}", request.Login);
                return BadRequest($"The User {request.Login} already exists.");
            }

            var user = new User(request);

            var createdUser = await userRepository.CreateAsync(user);

            logger.LogInformation("New user created: {UserId}", createdUser.Id);

            return CreatedAtAction(null, new UserResponse(createdUser));
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">The user ID to update</param>
        /// <param name="request">The user update request</param>
        /// <returns>The updated user response</returns>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "plain/text")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized, "plain/text")]
        public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UserRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await userRepository.ExistsAsync(id))
            {
                logger.LogWarning("Update attempt on non-existent user: {UserId}", id);
                return NotFound($"User Id {id} not found.");
            }

            var user = new User(request)
            {
                Id = id
            };

            var updatedUser = await userRepository.UpdateAsync(user);

            logger.LogInformation("User updated: {UserId}", id);

            return Ok(new UserResponse(updatedUser));
        }

        /// <summary>
        /// Retrieves a user by ID
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <returns>The user response</returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound, "plain/text")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized, "plain/text")]
        public async Task<ActionResult<UserResponse>> Get(int id)
        {
            var user = await userRepository.GetAsync(id);

            if (user == null)
            {
                logger.LogWarning("User not found: {UserId}", id);
                return NotFound($"User Id {id} not found.");
            }

            return Ok(new UserResponse(user));
        }
    }
}
