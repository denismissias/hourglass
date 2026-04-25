namespace Hourglass.Repository
{
    using Hourglass.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Entity Framework Core context for Hourglass database
    /// </summary>
    public class MySqlContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the MySqlContext class
        /// </summary>
        public MySqlContext() { }

        /// <summary>
        /// Initializes a new instance of the MySqlContext class with options
        /// </summary>
        /// <param name="options">The DbContext options</param>
        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the Users DbSet
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the Projects DbSet
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Gets or sets the Times DbSet
        /// </summary>
        public DbSet<Time> Times { get; set; }
    }

}
