using ReStyle.Application.DTOs;

namespace ReStyle.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetMyPurchasesAsync(int userId);
    Task<IEnumerable<TransactionDto>> GetMySalesAsync(int userId);
    Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
}
