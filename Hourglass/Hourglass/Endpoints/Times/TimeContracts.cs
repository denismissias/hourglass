using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hourglass.Endpoints.Times;

public sealed record CreateTimeRequest(
    [property: Required, Range(1, int.MaxValue), Description("Project identifier."), JsonPropertyName("project_id")] int ProjectId,
    [property: Required, Range(1, int.MaxValue), Description("User identifier."), JsonPropertyName("user_id")] int UserId,
    [property: Required, Description("Start date and time in UTC."), JsonPropertyName("started_at")] DateTime StartedAt,
    [property: Required, Description("End date and time in UTC."), JsonPropertyName("ended_at")] DateTime EndedAt);

public sealed record TimeResultDto(
    [property: Description("Time entry identifier."), JsonPropertyName("time_id")] int TimeId,
    [property: Description("Project identifier."), JsonPropertyName("project_id")] int ProjectId,
    [property: Description("User identifier."), JsonPropertyName("user_id")] int UserId,
    [property: Description("Start date and time in UTC."), JsonPropertyName("started_at")] DateTime StartedAt,
    [property: Description("End date and time in UTC."), JsonPropertyName("ended_at")] DateTime EndedAt);
