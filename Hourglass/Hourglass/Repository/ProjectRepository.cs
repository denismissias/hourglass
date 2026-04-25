using Hourglass.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hourglass.Repository
{
    public class ProjectRepository(MySqlContext context) : IProjectRepository
    {
        private readonly MySqlContext context = context;

        public async Task<bool> ExistsAsync(int projectId)
        {
            return await context.Projects.AsNoTracking().SingleOrDefaultAsync(project => project.Id == projectId) != null;
        }
    }
}
