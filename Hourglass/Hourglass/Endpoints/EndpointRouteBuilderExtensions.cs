using Hourglass.Endpoints.Authentication;
using Hourglass.Endpoints.Times;
using Hourglass.Endpoints.Users;

namespace Hourglass.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHourglassEndpoints(this IEndpointRouteBuilder app)
    {
        var apiGroup = app
            .MapGroup("/api/v{version:int}")
            .WithTags("Hourglass API");

        apiGroup.MapAuthenticationEndpoints();
        apiGroup.MapUserEndpoints();
        apiGroup.MapTimeEndpoints();

        return app;
    }
}
