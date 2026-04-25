namespace Hourglass.Repository.Interfaces
{
    /// <summary>
    /// Repository interface for Project entities
    /// </summary>
    public interface IProjectRepository
    {
        /// <summary>
        /// Checks if a project exists by ID
        /// </summary>
        /// <param name="projectId">The project ID to check</param>
        /// <returns>True if project exists, false otherwise</returns>
        Task<bool> ExistsAsync(int projectId);
    }
}
