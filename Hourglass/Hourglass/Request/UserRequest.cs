using System.ComponentModel.DataAnnotations;

namespace Hourglass.Request
{
    public class UserRequest
    {
        [StringLength(256, MinimumLength = 1)]
        public required string Name { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [StringLength(32, MinimumLength = 1)]
        public required string Login { get; set; }

        [StringLength(256, MinimumLength = 1)]
        public required string Password { get; set; }
    }
}
