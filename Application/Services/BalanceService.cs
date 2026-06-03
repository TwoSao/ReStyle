using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Entities;
using ReStyle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ReStyle.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly ReStyleDbContext _context;

    public BalanceService(ReStyleDbContext context) => _context = context;

    public async Task<(bool Success, string Message)> TopUpAsync(int userId, TopUpRequest request)
    {
        if (request.Amount <= 0) return (false, "Summa peab olema suurem kui 0.");
        if (string.IsNullOrWhiteSpace(request.CardNumber) || request.CardNumber.Length < 16)
            return (false, "Vale kaardinumbr." );

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return (false, "Kasutajat ei leitud.");

        user.Balance += request.Amount;

        _context.BalanceTopUps.Add(new BalanceTopUp
        {
            UserId = userId,
            Amount = request.Amount,
            CardNumberMasked = "**** **** **** " + request.CardNumber[^4..]
        });

        await _context.SaveChangesAsync();
        return (true, $"Saldo täiendati summaga {request.Amount:C}.");
    }

    public async Task<IEnumerable<BalanceTopUpDto>> GetTopUpHistoryAsync(int userId) =>
        await _context.BalanceTopUps
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new BalanceTopUpDto(t.TopUpId, t.Amount, t.CardNumberMasked, t.CreatedAt))
            .ToListAsync();
}
