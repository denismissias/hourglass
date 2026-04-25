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
    /// Time tracking controller
    /// </summary>
    [Route("api/v{version:apiVersion}/times")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TimeController(
        ITimeRepository timeRepository,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        ILogger<TimeController> logger) : ControllerBase
    {
        /// <summary>
        /// Creates a new time entry
        /// </summary>
        /// <param name="request">The time entry creation request</param>
        /// <returns>The created time entry response</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TimeResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "plain/text")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized, "plain/text")]
        public async Task<ActionResult<TimeResponse>> Create([FromBody] TimeRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await userRepository.ExistsAsync(request.UserId))
            {
                logger.LogWarning("Time creation attempt with non-existent user: {UserId}", request.UserId);
                return NotFound($"User Id {request.UserId} not found.");
            }

            if (!await projectRepository.ExistsAsync(request.ProjectId))
            {
                logger.LogWarning("Time creation attempt with non-existent project: {ProjectId}", request.ProjectId);
                return NotFound($"Project Id {request.ProjectId} not found.");
            }

            var time = new Time(request);

            var createdTime = await timeRepository.CreateAsync(time);

            logger.LogInformation("Time entry created: {TimeId} for user {UserId}", createdTime.Id, request.UserId);

            return CreatedAtAction(null, new TimeResponse(createdTime));
        }
    }
}
