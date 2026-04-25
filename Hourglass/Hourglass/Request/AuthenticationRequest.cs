using System.ComponentModel.DataAnnotations;

namespace Hourglass.Request
{
    public class AuthenticationRequest
    {
        [StringLength(32, MinimumLength = 3)]
        public required string Login { get; set; }

        [StringLength(256, MinimumLength = 3)]
        public required string Password { get; set; }
    }
}
