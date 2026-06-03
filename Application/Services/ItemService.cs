using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Entities;
using ReStyle.Core.Interfaces;

namespace ReStyle.Application.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepo;

    public ItemService(IItemRepository itemRepo) => _itemRepo = itemRepo;

    private static ItemDto ToDto(Item i) => new(
        i.ItemId, i.Title, i.Description, i.Price, i.ImagePath,
        i.Status, i.Category, i.Size, i.CreatedAt, i.UserId,
        i.User?.Username ?? string.Empty);

    public async Task<IEnumerable<ItemDto>> GetAvailableItemsAsync() =>
        (await _itemRepo.GetAvailableItemsAsync()).Select(ToDto);

    public async Task<IEnumerable<ItemDto>> GetMyItemsAsync(int userId) =>
        (await _itemRepo.GetItemsByUserAsync(userId)).Select(i => new ItemDto(
            i.ItemId, i.Title, i.Description, i.Price, i.ImagePath,
            i.Status, i.Category, i.Size, i.CreatedAt, i.UserId, string.Empty));

    public async Task<ItemDto?> GetItemByIdAsync(int itemId)
    {
        var item = await _itemRepo.GetItemWithDetailsAsync(itemId);
        return item == null ? null : ToDto(item);
    }

    public async Task<IEnumerable<ItemDto>> SearchItemsAsync(string query, string? category = null, decimal? minPrice = null, decimal? maxPrice = null) =>
        (await _itemRepo.SearchItemsAsync(query, category, minPrice, maxPrice)).Select(ToDto);

    public async Task<(bool Success, string Message, ItemDto? Item)> CreateItemAsync(int userId, CreateItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return (false, "Pealkiri on kohustuslik.", null);
        if (request.Price <= 0)
            return (false, "Hind peab olema suurem kui 0.", null);

        var item = new Item
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            ImagePath = request.ImagePath,
            Category = request.Category,
            Size = request.Size,
            UserId = userId
        };

        await _itemRepo.AddAsync(item);
        await _itemRepo.SaveChangesAsync();
        return (true, "Artikkel loodud.", new ItemDto(item.ItemId, item.Title, item.Description, item.Price, item.ImagePath, item.Status, item.Category, item.Size, item.CreatedAt, item.UserId, string.Empty));
    }

    public async Task<(bool Success, string Message)> UpdateItemAsync(int itemId, int userId, UpdateItemRequest request, bool isAdmin = false)
    {
        var item = await _itemRepo.GetByIdAsync(itemId);
        if (item == null) return (false, "Artiklit ei leitud.");
        if (!isAdmin && item.UserId != userId) return (false, "Pole õigusi.");

        item.Title = request.Title.Trim();
        item.Description = request.Description.Trim();
        item.Price = request.Price;
        item.ImagePath = request.ImagePath;
        item.Category = request.Category;
        item.Size = request.Size;

        await _itemRepo.UpdateAsync(item);
        await _itemRepo.SaveChangesAsync();
        return (true, "Artikkel uuendatud.");
    }

    public async Task<(bool Success, string Message)> DeleteItemAsync(int itemId, int userId, bool isAdmin = false)
    {
        var item = await _itemRepo.GetByIdAsync(itemId);
        if (item == null) return (false, "Artiklit ei leitud.");
        if (!isAdmin && item.UserId != userId) return (false, "Pole õigusi.");

        await _itemRepo.DeleteAsync(item);
        await _itemRepo.SaveChangesAsync();
        return (true, "Artikkel kustutatud.");
    }
}
