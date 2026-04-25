using System.ComponentModel.DataAnnotations.Schema;

namespace Hourglass.Models
{
    public class Project
    {
        [Column("project_id")]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<User> Users { get; set; }

        public IEnumerable<Time> Times { get; set; }
    }
}
