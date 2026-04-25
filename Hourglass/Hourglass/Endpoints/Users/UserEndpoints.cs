using Hourglass.Endpoints.Common;
using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Hourglass.Endpoints.Users;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder versionGroup)
    {
        var usersGroup = versionGroup
            .MapGroup("/users")
            .RequireAuthorization()
            .WithTags("Users");

        usersGroup.MapPost(string.Empty, CreateAsync)
            .AddEndpointFilter<ValidationEndpointFilter<CreateOrUpdateUserRequest>>()
            .WithName("CreateUser")
            .WithSummary("Create user")
            .WithDescription("Creates a new user with a hashed password.")
            .Accepts<CreateOrUpdateUserRequest>("application/json")
            .Produces<UserResultDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        usersGroup.MapPut("/{id:int}", UpdateAsync)
            .AddEndpointFilter<ValidationEndpointFilter<CreateOrUpdateUserRequest>>()
            .WithName("UpdateUser")
            .WithSummary("Update user")
            .WithDescription("Updates an existing user by identifier.")
            .Accepts<CreateOrUpdateUserRequest>("application/json")
            .Produces<UserResultDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        usersGroup.MapGet("/{id:int}", GetAsync)
            .WithName("GetUserById")
            .WithSummary("Get user by id")
            .WithDescription("Retrieves a user by identifier.")
            .Produces<UserResultDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return usersGroup;
    }

    private static async Task<Results<Created<UserResultDto>, BadRequest<ProblemDetails>>> CreateAsync(
        int version,
        CreateOrUpdateUserRequest request,
        IUserRepository userRepository,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("UsersEndpoint");

        if (await userRepository.ExistsAsync(request.Login))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Duplicate user",
                Detail = $"The user '{request.Login}' already exists.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var createdUser = await userRepository.CreateAsync(new User
        {
            Login = request.Login,
            Name = request.Name,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        });

        logger.LogInformation("User created: {UserId}", createdUser.Id);

        var response = new UserResultDto(createdUser.Id, createdUser.Login, createdUser.Name, createdUser.Email);
        return TypedResults.Created($"/api/v{version}/users/{createdUser.Id}", response);
    }

    private static async Task<Results<Ok<UserResultDto>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> UpdateAsync(
        int id,
        CreateOrUpdateUserRequest request,
        IUserRepository userRepository,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("UsersEndpoint");

        if (!await userRepository.ExistsAsync(id))
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"User Id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var updated = await userRepository.UpdateAsync(new User
        {
            Id = id,
            Login = request.Login,
            Name = request.Name,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        });

        logger.LogInformation("User updated: {UserId}", id);

        return TypedResults.Ok(new UserResultDto(updated.Id, updated.Login, updated.Name, updated.Email));
    }

    private static async Task<Results<Ok<UserResultDto>, NotFound<ProblemDetails>>> GetAsync(
        int id,
        IUserRepository userRepository,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("UsersEndpoint");
        var user = await userRepository.GetAsync(id);

        if (user is null)
        {
            logger.LogWarning("User not found: {UserId}", id);
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"User Id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return TypedResults.Ok(new UserResultDto(user.Id, user.Login, user.Name, user.Email));
    }
}
