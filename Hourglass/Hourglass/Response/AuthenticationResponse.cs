namespace Hourglass.Response
{
    public class AuthenticationResponse
    {
        public string Token { get; init; }

        public UserResponse User { get; init; }
    }
}
