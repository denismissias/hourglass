using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hourglass.Endpoints.Users;

public sealed record CreateOrUpdateUserRequest(
    [property: Required, StringLength(256, MinimumLength = 1), Description("Full user name.")] string Name,
    [property: Required, EmailAddress, Description("User e-mail address.")] string Email,
    [property: Required, StringLength(32, MinimumLength = 1), Description("Unique login name.")] string Login,
    [property: Required, StringLength(256, MinimumLength = 1), Description("Plain-text password to be hashed.")] string Password);

public sealed record UserResultDto(
    [property: Description("User identifier.")] int UserId,
    [property: Description("User login.")] string Login,
    [property: Description("User name.")] string Name,
    [property: Description("User e-mail address.")] string Email);
