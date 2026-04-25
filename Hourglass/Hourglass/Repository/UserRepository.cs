using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hourglass.Repository
{
    public class UserRepository(MySqlContext context) : IUserRepository
    {
        private readonly MySqlContext _context = context;

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> ExistsAsync(string login)
        {
            return await _context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Login == login) != null;
        }

        public async Task<bool> ExistsAsync(int userId)
        {
            return await _context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId) != null;

        }

        public async Task<User> GetAsync(string login)
        {
            return await _context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Login == login);
        }

        public async Task<User> GetAsync(int userId)
        {
            return await _context.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId);
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return user;
        }
    }
}
