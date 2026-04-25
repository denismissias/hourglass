using Hourglass.Models;

namespace Hourglass.Repository.Interfaces
{
    /// <summary>
    /// Repository interface for Time entities
    /// </summary>
    public interface ITimeRepository
    {
        /// <summary>
        /// Creates a new time entry in the database
        /// </summary>
        /// <param name="time">The time entry to create</param>
        /// <returns>The created time entry</returns>
        Task<Time> CreateAsync(Time time);

        /// <summary>
        /// Retrieves all time entries for a specific project
        /// </summary>
        /// <param name="projectId">The project ID to filter by</param>
        /// <returns>A collection of time entries for the project</returns>
        Task<IEnumerable<Time>> GetAsync(int projectId);

        /// <summary>
        /// Updates an existing time entry in the database
        /// </summary>
        /// <param name="time">The time entry with updated values</param>
        /// <returns>The updated time entry</returns>
        Task<Time> UpdateAsync(Time time);
    }
}
