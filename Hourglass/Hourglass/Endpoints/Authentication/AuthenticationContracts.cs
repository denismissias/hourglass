using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hourglass.Endpoints.Authentication;

public sealed record AuthenticateRequest(
    [property: Required, StringLength(32, MinimumLength = 3), Description("User login.")] string Login,
    [property: Required, StringLength(256, MinimumLength = 3), Description("User password.")] string Password);

public sealed record UserDto(
    [property: Description("User identifier.")] int UserId,
    [property: Description("User login.")] string Login,
    [property: Description("User name.")] string Name,
    [property: Description("User e-mail address.")] string Email);

public sealed record AuthenticationResultDto(
    [property: Description("Generated JWT bearer token.")] string Token,
    [property: Description("Authenticated user metadata.")] UserDto User);
