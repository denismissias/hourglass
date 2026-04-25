using Hourglass.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hourglass.Repository
{
    /// <summary>
    /// Repository for managing Project entities in the database
    /// </summary>
    public class ProjectRepository(MySqlContext context) : IProjectRepository
    {
        /// <summary>
        /// Checks if a project exists by ID
        /// </summary>
        /// <param name="projectId">The project ID to check</param>
        /// <returns>True if project exists, false otherwise</returns>
        public async Task<bool> ExistsAsync(int projectId)
        {
            return await context.Projects.AsNoTracking().SingleOrDefaultAsync(project => project.Id == projectId) != null;
        }
    }
}
