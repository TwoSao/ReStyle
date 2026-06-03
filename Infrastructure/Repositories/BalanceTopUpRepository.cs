using Microsoft.EntityFrameworkCore;
using ReStyle.Core.Entities;
using ReStyle.Core.Interfaces;
using ReStyle.Infrastructure.Data;

namespace ReStyle.Infrastructure.Repositories;

public class BalanceTopUpRepository : Repository<BalanceTopUp>, IBalanceTopUpRepository
{
    public BalanceTopUpRepository(ReStyleDbContext context) : base(context) { }

    public async Task<IEnumerable<BalanceTopUp>> GetByUserAsync(int userId) =>
        await _dbSet.Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
}
