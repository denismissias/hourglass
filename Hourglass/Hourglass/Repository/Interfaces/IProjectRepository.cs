using Hourglass.Models;

namespace Hourglass.Repository.Interfaces
{
    public interface IProjectRepository
    {
        Task<bool> ExistsAsync(int projectId);
    }
}
