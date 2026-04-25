using System.ComponentModel.DataAnnotations.Schema;

namespace Hourglass.Models
{
    public class User
    {
        public User()
        {

        }

        [Column("user_id")]
        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public IEnumerable<Project> Projects { get; set; }

        public IEnumerable<Time> Times { get; set; }
    }
}
