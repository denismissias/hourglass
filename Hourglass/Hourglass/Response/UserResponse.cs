using Hourglass.Models;
using System.Text.Json.Serialization;

namespace Hourglass.Response
{
    public class UserResponse(User user)
    {
        [JsonPropertyName("user_id")]
        public int Id { get; init; } = user.Id;

        public string Login { get; init; } = user.Login;

        public string Name { get; init; } = user.Name;

        public string Email { get; init; } = user.Email;
    }
}
