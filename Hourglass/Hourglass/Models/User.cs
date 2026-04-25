using Hourglass.Request;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hourglass.Models
{
    public class User
    {
        public User()
        {

        }
        public User(UserRequest request)
        {
            Login = request.Login;
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            Name = request.Name;
            Email = request.Email;
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
