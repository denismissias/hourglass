using Hourglass.Models;
using System.Text.Json.Serialization;

namespace Hourglass.Response
{
    public class TimeResponse(Time time)
    {
        [JsonPropertyName("time_id")]
        public int Id { get; init; } = time.Id;

        [JsonPropertyName("started_at")]
        public DateTime StartedAt { get; init; } = time.StartedAt;

        [JsonPropertyName("ended_at")]
        public DateTime EndedAt { get; init; } = time.EndedAt;
    }
}
