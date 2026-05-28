using ReStyle.Core.Entities;

namespace ReStyle.Core.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByBuyerAsync(int buyerId);
    Task<IEnumerable<Transaction>> GetBySellerAsync(int sellerId);
    Task<IEnumerable<Transaction>> GetAllWithDetailsAsync();
}
