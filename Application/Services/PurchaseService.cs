using Microsoft.EntityFrameworkCore;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Entities;
using ReStyle.Core.Enums;
using ReStyle.Infrastructure.Data;

namespace ReStyle.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly ReStyleDbContext _context;
    private readonly IAuthService _authService;

    public PurchaseService(ReStyleDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<(bool Success, string Message)> PurchaseItemAsync(int buyerId, int itemId)
    {
        var item = await _context.Items.Include(i => i.User).FirstOrDefaultAsync(i => i.ItemId == itemId);
        if (item == null) return (false, "Artiklit ei leitud.");
        if (item.Status != ItemStatus.Available) return (false, "Artikkel ei ole enam saadaval.");
        if (item.UserId == buyerId) return (false, "Sa ei saa osta oma artiklit.");

        var buyer = await _context.Users.FindAsync(buyerId);
        if (buyer == null) return (false, "Ostjat ei leitud.");

        // Use in-memory balance from CurrentUser — it reflects top-ups done in this session
        var effectiveBalance = _authService.CurrentUser?.UserId == buyerId
            ? _authService.CurrentUser.Balance
            : buyer.Balance;

        if (effectiveBalance < item.Price) return (false, "Saldo ei ole piisav.");

        var seller = await _context.Users.FindAsync(item.UserId);
        if (seller == null) return (false, "Müüjat ei leitud.");

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            buyer.Balance = effectiveBalance - item.Price;
            seller.Balance += item.Price;
            item.Status = ItemStatus.Sold;

            _context.Transactions.Add(new Transaction
            {
                BuyerId = buyerId,
                SellerId = item.UserId,
                ItemId = itemId,
                Amount = item.Price
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            // Sync in-memory session
            if (_authService.CurrentUser?.UserId == buyerId)
            {
                _authService.CurrentUser.Balance = buyer.Balance;
                _authService.NotifyBalanceChanged();
            }

            return (true, "Ost õnnestus!");
        }
        catch
        {
            await tx.RollbackAsync();
            return (false, "Ost ebaõnnestus. Palun proovi uuesti.");
        }
    }
}
