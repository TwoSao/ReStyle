using ReStyle.Application.DTOs;

namespace ReStyle.Application.Interfaces;

public interface IBalanceService
{
    Task<(bool Success, string Message)> TopUpAsync(int userId, TopUpRequest request);
    Task<IEnumerable<BalanceTopUpDto>> GetTopUpHistoryAsync(int userId);
}
