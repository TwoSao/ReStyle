using ReStyle.Core.Entities;

namespace ReStyle.Core.Interfaces;

public interface IBalanceTopUpRepository : IRepository<BalanceTopUp>
{
    Task<IEnumerable<BalanceTopUp>> GetByUserAsync(int userId);
}
