using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Entities;
using ReStyle.Core.Interfaces;

namespace ReStyle.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly IUserRepository _userRepo;
    private readonly IBalanceTopUpRepository _topUpRepo;

    public BalanceService(IUserRepository userRepo, IBalanceTopUpRepository topUpRepo)
    {
        _userRepo = userRepo;
        _topUpRepo = topUpRepo;
    }

    public async Task<(bool Success, string Message)> TopUpAsync(int userId, TopUpRequest request)
    {
        if (request.Amount <= 0) return (false, "Amount must be greater than 0.");
        if (string.IsNullOrWhiteSpace(request.CardNumber) || request.CardNumber.Length < 16)
            return (false, "Invalid card number.");

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return (false, "User not found.");

        user.Balance += request.Amount;
        var masked = "**** **** **** " + request.CardNumber[^4..];

        var topUp = new BalanceTopUp
        {
            UserId = userId,
            Amount = request.Amount,
            CardNumberMasked = masked
        };

        await _topUpRepo.AddAsync(topUp);
        await _topUpRepo.SaveChangesAsync();
        return (true, $"Balance topped up by {request.Amount:C}.");
    }

    public async Task<IEnumerable<BalanceTopUpDto>> GetTopUpHistoryAsync(int userId) =>
        (await _topUpRepo.GetByUserAsync(userId))
            .Select(t => new BalanceTopUpDto(t.TopUpId, t.Amount, t.CardNumberMasked, t.CreatedAt));
}
