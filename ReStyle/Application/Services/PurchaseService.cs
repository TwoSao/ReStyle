using Microsoft.EntityFrameworkCore;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Entities;
using ReStyle.Core.Enums;
using ReStyle.Core.Interfaces;
using ReStyle.Infrastructure.Data;

namespace ReStyle.Application.Services;

/// <summary>
/// Handles atomic purchase: debit buyer, credit seller, create transaction, mark item sold.
/// </summary>
public class PurchaseService : IPurchaseService
{
    private readonly IUserRepository _userRepo;
    private readonly IItemRepository _itemRepo;
    private readonly ITransactionRepository _transactionRepo;
    private readonly ReStyleDbContext _context;

    public PurchaseService(IUserRepository userRepo, IItemRepository itemRepo,
        ITransactionRepository transactionRepo, ReStyleDbContext context)
    {
        _userRepo = userRepo;
        _itemRepo = itemRepo;
        _transactionRepo = transactionRepo;
        _context = context;
    }

    public async Task<(bool Success, string Message)> PurchaseItemAsync(int buyerId, int itemId)
    {
        var item = await _itemRepo.GetItemWithDetailsAsync(itemId);
        if (item == null) return (false, "Item not found.");
        if (item.Status != ItemStatus.Available) return (false, "Item is no longer available.");
        if (item.UserId == buyerId) return (false, "You cannot buy your own item.");

        var buyer = await _userRepo.GetByIdAsync(buyerId);
        if (buyer == null) return (false, "Buyer not found.");
        if (buyer.Balance < item.Price) return (false, "Insufficient balance.");

        var seller = await _userRepo.GetByIdAsync(item.UserId);
        if (seller == null) return (false, "Seller not found.");

        // Atomic transaction
        await using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            buyer.Balance -= item.Price;
            seller.Balance += item.Price;
            item.Status = ItemStatus.Sold;

            var transaction = new Transaction
            {
                BuyerId = buyerId,
                SellerId = item.UserId,
                ItemId = itemId,
                Amount = item.Price
            };

            await _transactionRepo.AddAsync(transaction);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
            return (true, "Purchase successful!");
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            return (false, "Purchase failed. Please try again.");
        }
    }
}
