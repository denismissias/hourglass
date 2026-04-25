using System.ComponentModel.DataAnnotations;

namespace Hourglass.Endpoints.Common;

/// <summary>
/// Endpoint filter that validates request DTOs based on DataAnnotations.
/// </summary>
public sealed class ValidationEndpointFilter<TRequest> : IEndpointFilter where TRequest : class
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
        {
            return next(context);
        }

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);
        var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        if (isValid)
        {
            return next(context);
        }

        var errors = validationResults
            .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty), (result, member) => new { member, result.ErrorMessage })
            .GroupBy(x => x.member)
            .ToDictionary(
                group => string.IsNullOrWhiteSpace(group.Key) ? "request" : group.Key,
                group => group.Select(x => x.ErrorMessage ?? "Invalid value.").ToArray());

        return ValueTask.FromResult<object?>(TypedResults.ValidationProblem(errors));
    }
}
