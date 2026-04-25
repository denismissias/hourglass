using Hourglass.Endpoints.Common;
using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Hourglass.Endpoints.Times;

public static class TimeEndpoints
{
    public static RouteGroupBuilder MapTimeEndpoints(this RouteGroupBuilder versionGroup)
    {
        var timesGroup = versionGroup
            .MapGroup("/times")
            .RequireAuthorization()
            .WithTags("Times");

        timesGroup.MapPost(string.Empty, CreateAsync)
            .AddEndpointFilter<ValidationEndpointFilter<CreateTimeRequest>>()
            .WithName("CreateTimeEntry")
            .WithSummary("Create time entry")
            .WithDescription("Creates a time entry linked to an existing user and project.")
            .Accepts<CreateTimeRequest>("application/json")
            .Produces<TimeResultDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return timesGroup;
    }

    private static async Task<Results<Created<TimeResultDto>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> CreateAsync(
        int version,
        CreateTimeRequest request,
        ITimeRepository timeRepository,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("TimesEndpoint");

        if (request.EndedAt <= request.StartedAt)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid time range",
                Detail = "ended_at must be greater than started_at.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!await userRepository.ExistsAsync(request.UserId))
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"User Id {request.UserId} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        if (!await projectRepository.ExistsAsync(request.ProjectId))
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Project not found",
                Detail = $"Project Id {request.ProjectId} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var created = await timeRepository.CreateAsync(new Time
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            StartedAt = request.StartedAt,
            EndedAt = request.EndedAt
        });

        logger.LogInformation("Time entry created: {TimeId}", created.Id);

        var response = new TimeResultDto(created.Id, created.ProjectId, created.UserId, created.StartedAt, created.EndedAt);
        return TypedResults.Created($"/api/v{version}/times/{created.Id}", response);
    }
}
