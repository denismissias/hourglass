using System.Text.Json.Serialization;

namespace Hourglass.Request
{
    public class TimeRequest
    {
        [JsonPropertyName("project_id")]
        public required int ProjectId { get; set; }

        [JsonPropertyName("user_id")]
        public required int UserId { get; set; }

        [JsonPropertyName("started_at")]
        public required DateTime StartedAt { get; set; }

        [JsonPropertyName("ended_at")]
        public required DateTime EndedAt { get; set; }
    }
}
