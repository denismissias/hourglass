using Hourglass.Models;

namespace Hourglass.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(string login);

        Task<bool> ExistsAsync(int userId);

        Task<User> GetAsync(string login);

        Task<User> GetAsync(int userId);

        Task<User> CreateAsync(User user);

        Task<User> UpdateAsync(User time);
    }
}
