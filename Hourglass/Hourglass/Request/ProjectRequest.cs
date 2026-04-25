using System.ComponentModel.DataAnnotations;

namespace Hourglass.Request
{
    public class ProjectRequest
    {
        [StringLength(64, MinimumLength = 1)]
        public required string Title { get; set; }

        [StringLength(512, MinimumLength = 1)]
        public string Description { get; set; }

        public IEnumerable<int> UsersIds { get; set; }
    }
}
