using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hourglass.Repository
{
    /// <summary>
    /// Repository for managing Time entities in the database
    /// </summary>
    public class TimeRepository(MySqlContext context, ILogger<TimeRepository> logger) : ITimeRepository
    {
        /// <summary>
        /// Creates a new time entry in the database
        /// </summary>
        /// <param name="time">The time entry to create</param>
        /// <returns>The created time entry</returns>
        public async Task<Time> CreateAsync(Time time)
        {
            ArgumentNullException.ThrowIfNull(time);

            await context.Times.AddAsync(time);
            await context.SaveChangesAsync();

            logger.LogInformation("Time entry created with Id: {TimeId}", time.Id);

            return time;
        }

        /// <summary>
        /// Retrieves all time entries for a specific project
        /// </summary>
        /// <param name="projectId">The project ID to filter by</param>
        /// <returns>A collection of time entries for the project</returns>
        public async Task<IEnumerable<Time>> GetAsync(int projectId)
        {
            return await context.Times.AsNoTracking().Where(time => time.ProjectId == projectId).ToListAsync();
        }

        /// <summary>
        /// Updates an existing time entry in the database
        /// </summary>
        /// <param name="time">The time entry with updated values</param>
        /// <returns>The updated time entry</returns>
        public async Task<Time> UpdateAsync(Time time)
        {
            ArgumentNullException.ThrowIfNull(time);

            context.Entry(time).State = EntityState.Modified;
            await context.SaveChangesAsync();

            logger.LogInformation("Time entry updated with Id: {TimeId}", time.Id);

            return time;
        }
    }
}