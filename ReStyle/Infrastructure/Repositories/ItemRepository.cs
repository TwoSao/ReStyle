using Microsoft.EntityFrameworkCore;
using ReStyle.Core.Entities;
using ReStyle.Core.Enums;
using ReStyle.Core.Interfaces;
using ReStyle.Infrastructure.Data;

namespace ReStyle.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(ReStyleDbContext context) : base(context) { }

    public async Task<IEnumerable<Item>> GetAvailableItemsAsync() =>
        await _dbSet.Include(i => i.User)
                    .Where(i => i.Status == ItemStatus.Available)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();

    public async Task<IEnumerable<Item>> GetItemsByUserAsync(int userId) =>
        await _dbSet.Where(i => i.UserId == userId)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();

    public async Task<IEnumerable<Item>> SearchItemsAsync(string query, string? category = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        var q = _dbSet.Include(i => i.User)
                      .Where(i => i.Status == ItemStatus.Available);

        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(i => i.Title.ToLower().Contains(query.ToLower()) ||
                              i.Description.ToLower().Contains(query.ToLower()));

        if (!string.IsNullOrWhiteSpace(category))
            q = q.Where(i => i.Category == category);

        if (minPrice.HasValue) q = q.Where(i => i.Price >= minPrice.Value);
        if (maxPrice.HasValue) q = q.Where(i => i.Price <= maxPrice.Value);

        return await q.OrderByDescending(i => i.CreatedAt).ToListAsync();
    }

    public async Task<Item?> GetItemWithDetailsAsync(int itemId) =>
        await _dbSet.Include(i => i.User)
                    .FirstOrDefaultAsync(i => i.ItemId == itemId);
}
