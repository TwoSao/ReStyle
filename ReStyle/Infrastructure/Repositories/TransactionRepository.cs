using Microsoft.EntityFrameworkCore;
using ReStyle.Core.Entities;
using ReStyle.Core.Interfaces;
using ReStyle.Infrastructure.Data;

namespace ReStyle.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(ReStyleDbContext context) : base(context) { }

    public async Task<IEnumerable<Transaction>> GetByBuyerAsync(int buyerId) =>
        await _dbSet.Include(t => t.Item)
                    .Include(t => t.Seller)
                    .Where(t => t.BuyerId == buyerId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();

    public async Task<IEnumerable<Transaction>> GetBySellerAsync(int sellerId) =>
        await _dbSet.Include(t => t.Item)
                    .Include(t => t.Buyer)
                    .Where(t => t.SellerId == sellerId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();

    public async Task<IEnumerable<Transaction>> GetAllWithDetailsAsync() =>
        await _dbSet.Include(t => t.Item)
                    .Include(t => t.Buyer)
                    .Include(t => t.Seller)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();
}
