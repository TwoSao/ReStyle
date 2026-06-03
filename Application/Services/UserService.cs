using Microsoft.EntityFrameworkCore;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Interfaces;
using ReStyle.Infrastructure.Data;

namespace ReStyle.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly ReStyleDbContext _context;

    public UserService(IUserRepository userRepo, ReStyleDbContext context)
    {
        _userRepo = userRepo;
        _context = context;
    }

    private static UserDto ToDto(Core.Entities.User u) =>
        new(u.UserId, u.Username, u.Email, u.Balance, u.Role, u.IsBlocked, u.CreatedAt);

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        return user == null ? null : ToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync() =>
        (await _userRepo.GetAllUsersAsync())
        .Where(u => u.Role != Core.Enums.UserRole.Admin)
        .Select(ToDto);

    public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return (false, "Kasutajat ei leitud.");

        var existing = await _userRepo.GetByUsernameAsync(request.Username);
        if (existing != null && existing.UserId != userId)
            return (false, "Kasutajanimi on juba võetud.");

        user.Username = request.Username.Trim();
        user.Email = request.Email.Trim().ToLower();
        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return (true, "Profiil uuendatud.");
    }

    public async Task<(bool Success, string Message)> BlockUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return (false, "Kasutajat ei leitud.");
        user.IsBlocked = true;
        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return (true, "Kasutaja blokeeritud.");
    }

    public async Task<(bool Success, string Message)> UnblockUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return (false, "Kasutajat ei leitud.");
        user.IsBlocked = false;
        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return (true, "Kasutaja blokeering eemaldatud.");
    }

    public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return (false, "Kasutajat ei leitud.");

        // All operations on the same DbContext so one SaveChangesAsync covers everything
        // Order: transactions first (FK Restrict on ItemId, BuyerId, SellerId), then items, then user
        var transactions = await _context.Transactions
            .Where(t => t.BuyerId == userId || t.SellerId == userId
                     || _context.Items.Where(i => i.UserId == userId).Select(i => i.ItemId).Contains(t.ItemId))
            .ToListAsync();
        _context.Transactions.RemoveRange(transactions);

        var items = await _context.Items.Where(i => i.UserId == userId).ToListAsync();
        _context.Items.RemoveRange(items);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return (true, "Kasutaja kustutatud.");
    }
}
