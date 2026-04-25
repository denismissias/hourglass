using Hourglass.Models;

namespace Hourglass.Repository.Interfaces
{
    public interface ITimeRepository
    {
        Task<Time> CreateAsync(Time time);

        Task<IEnumerable<Time>> GetAsync(int projectId);

        Task<Time> UpdateAsync(Time time);
    }
}
