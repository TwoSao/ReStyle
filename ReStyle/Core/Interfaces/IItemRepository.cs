using ReStyle.Core.Entities;

namespace ReStyle.Core.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    Task<IEnumerable<Item>> GetAvailableItemsAsync();
    Task<IEnumerable<Item>> GetItemsByUserAsync(int userId);
    Task<IEnumerable<Item>> SearchItemsAsync(string query, string? category = null, decimal? minPrice = null, decimal? maxPrice = null);
    Task<Item?> GetItemWithDetailsAsync(int itemId);
}
