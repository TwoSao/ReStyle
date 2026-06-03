using ReStyle.Application.DTOs;

namespace ReStyle.Application.Interfaces;

public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAvailableItemsAsync();
    Task<IEnumerable<ItemDto>> GetMyItemsAsync(int userId);
    Task<ItemDto?> GetItemByIdAsync(int itemId);
    Task<IEnumerable<ItemDto>> SearchItemsAsync(string query, string? category = null, decimal? minPrice = null, decimal? maxPrice = null);
    Task<(bool Success, string Message, ItemDto? Item)> CreateItemAsync(int userId, CreateItemRequest request);
    Task<(bool Success, string Message)> UpdateItemAsync(int itemId, int userId, UpdateItemRequest request, bool isAdmin = false);
    Task<(bool Success, string Message)> DeleteItemAsync(int itemId, int userId, bool isAdmin = false);
}
