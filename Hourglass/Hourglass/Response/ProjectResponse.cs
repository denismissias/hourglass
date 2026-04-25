using System.Text.Json.Serialization;

namespace Hourglass.Response
{
    public class ProjectResponse
    {
        [JsonPropertyName("project_id")]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<int> UsersIds { get; set; }
    }
}
