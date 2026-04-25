namespace Hourglass.Repository
{
    using Hourglass.Models;
    using Microsoft.EntityFrameworkCore;

    public class MySqlContext : DbContext
    {
        public MySqlContext() { }

        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Time> Times { get; set; }
    }

}
