using Hourglass.Endpoints.Common;
using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Hourglass.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Hourglass.Endpoints.Authentication;

public static class AuthenticationEndpoints
{
    public static RouteGroupBuilder MapAuthenticationEndpoints(this RouteGroupBuilder versionGroup)
    {
        var authGroup = versionGroup
            .MapGroup(string.Empty)
            .WithTags("Authentication");

        authGroup.MapPost("/authenticate", AuthenticateAsync)
            .AddEndpointFilter<ValidationEndpointFilter<AuthenticateRequest>>()
            .WithName("AuthenticateUser")
            .WithSummary("Authenticate a user")
            .WithDescription("Validates user credentials and returns a bearer JWT token when credentials are valid.")
            .Accepts<AuthenticateRequest>("application/json")
            .Produces<AuthenticationResultDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return authGroup;
    }

    private static async Task<Results<Ok<AuthenticationResultDto>, BadRequest<ProblemDetails>>> AuthenticateAsync(
        AuthenticateRequest request,
        IUserRepository userRepository,
        ITokenService tokenService,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("AuthenticationEndpoint");
        var user = await userRepository.GetAsync(request.Login);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            logger.LogWarning("Failed authentication attempt for login: {Login}", request.Login);
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid credentials",
                Detail = "Login or password is invalid.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var token = tokenService.Generate(user);
        var dto = new AuthenticationResultDto(
            token,
            new UserDto(user.Id, user.Login, user.Name, user.Email));

        return TypedResults.Ok(dto);
    }
}
