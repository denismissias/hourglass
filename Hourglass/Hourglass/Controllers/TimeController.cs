using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Hourglass.Request;
using Hourglass.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hourglass.Controllers
{
    [Route("api/v{version:apiVersion}/times")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TimeController(ITimeRepository timeRepository, IUserRepository userRepository, IProjectRepository projectRepository) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(TimeResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest, "plain/text")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized, "plain/text")]
        public async Task<ActionResult<TimeResponse>> Create([FromBody] TimeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await userRepository.ExistsAsync(request.UserId))
                return NotFound($"User Id {request.UserId} not found.");

            if (!await projectRepository.ExistsAsync(request.ProjectId))
                return NotFound($"Project Id {request.ProjectId} not found.");

            var time = new Time(request);

            return CreatedAtAction(null, new TimeResponse(await timeRepository.CreateAsync(time)));
        }
    }
}
