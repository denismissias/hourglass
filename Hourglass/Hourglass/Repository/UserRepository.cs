using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hourglass.Repository
{
    /// <summary>
    /// Repository for managing User entities in the database
    /// </summary>
    public class UserRepository(MySqlContext context, ILogger<UserRepository> logger) : IUserRepository
    {
        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        /// <param name="user">The user to create</param>
        /// <returns>The created user</returns>
        public async Task<User> CreateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            logger.LogInformation("User created with Id: {UserId}, Login: {UserLogin}", user.Id, user.Login);

            return user;
        }

        /// <summary>
        /// Checks if a user exists by login
        /// </summary>
        /// <param name="login">The user login to check</param>
        /// <returns>True if user exists, false otherwise</returns>
        public async Task<bool> ExistsAsync(string login)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(login);

            return await context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Login == login) != null;
        }

        /// <summary>
        /// Checks if a user exists by ID
        /// </summary>
        /// <param name="userId">The user ID to check</param>
        /// <returns>True if user exists, false otherwise</returns>
        public async Task<bool> ExistsAsync(int userId)
        {
            return await context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId) != null;
        }

        /// <summary>
        /// Retrieves a user by login
        /// </summary>
        /// <param name="login">The user login</param>
        /// <returns>The user if found, null otherwise</returns>
        public async Task<User?> GetAsync(string login)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(login);

            return await context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Login == login);
        }

        /// <summary>
        /// Retrieves a user by ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user if found, null otherwise</returns>
        public async Task<User?> GetAsync(int userId)
        {
            return await context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId);
        }

        /// <summary>
        /// Updates an existing user in the database
        /// </summary>
        /// <param name="user">The user with updated values</param>
        /// <returns>The updated user</returns>
        public async Task<User> UpdateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();

            logger.LogInformation("User updated with Id: {UserId}", user.Id);

            return user;
        }
    }
}
