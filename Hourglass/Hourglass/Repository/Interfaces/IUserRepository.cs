using Hourglass.Models;

namespace Hourglass.Repository.Interfaces
{
    /// <summary>
    /// Repository interface for User entities
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Checks if a user exists by login
        /// </summary>
        /// <param name="login">The user login to check</param>
        /// <returns>True if user exists, false otherwise</returns>
        Task<bool> ExistsAsync(string login);

        /// <summary>
        /// Checks if a user exists by ID
        /// </summary>
        /// <param name="userId">The user ID to check</param>
        /// <returns>True if user exists, false otherwise</returns>
        Task<bool> ExistsAsync(int userId);

        /// <summary>
        /// Retrieves a user by login
        /// </summary>
        /// <param name="login">The user login</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetAsync(string login);

        /// <summary>
        /// Retrieves a user by ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetAsync(int userId);

        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        /// <param name="user">The user to create</param>
        /// <returns>The created user</returns>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Updates an existing user in the database
        /// </summary>
        /// <param name="user">The user with updated values</param>
        /// <returns>The updated user</returns>
        Task<User> UpdateAsync(User user);
    }
}
