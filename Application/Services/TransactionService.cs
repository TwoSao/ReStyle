using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Interfaces;

namespace ReStyle.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepo;

    public TransactionService(ITransactionRepository transactionRepo) => _transactionRepo = transactionRepo;

    private static TransactionDto ToDto(Core.Entities.Transaction t) => new(
        t.TransactionId, t.BuyerId, t.Buyer?.Username ?? string.Empty,
        t.SellerId, t.Seller?.Username ?? string.Empty,
        t.ItemId, t.Item?.Title ?? string.Empty,
        t.Amount, t.TransactionDate);

    public async Task<IEnumerable<TransactionDto>> GetMyPurchasesAsync(int userId) =>
        (await _transactionRepo.GetByBuyerAsync(userId)).Select(ToDto);

    public async Task<IEnumerable<TransactionDto>> GetMySalesAsync(int userId) =>
        (await _transactionRepo.GetBySellerAsync(userId)).Select(ToDto);

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync() =>
        (await _transactionRepo.GetAllWithDetailsAsync()).Select(ToDto);
}
