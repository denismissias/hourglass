using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hourglass.Repository
{
    public class TimeRepository(MySqlContext context) : ITimeRepository
    {
        private readonly MySqlContext context = context;

        public async Task<Time> CreateAsync(Time time)
        {
            await context.Times.AddAsync(time);

            await context.SaveChangesAsync();

            return time;
        }

        public async Task<IEnumerable<Time>> GetAsync(int projectId)
        {
            return await context.Times.AsNoTracking().Where(time => time.ProjectId == projectId).ToListAsync();
        }

        public Task<Time> UpdateAsync(Time time)
        {
            throw new NotImplementedException();
        }
    }
}