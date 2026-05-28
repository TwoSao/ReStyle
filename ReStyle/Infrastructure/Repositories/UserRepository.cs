using Microsoft.EntityFrameworkCore;
using ReStyle.Core.Entities;
using ReStyle.Core.Interfaces;
using ReStyle.Infrastructure.Data;

namespace ReStyle.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ReStyleDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

    public async Task<IEnumerable<User>> GetAllUsersAsync() =>
        await _dbSet.OrderByDescending(u => u.CreatedAt).ToListAsync();
}
