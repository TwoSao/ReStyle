using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Interfaces;

namespace ReStyle.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo) => _userRepo = userRepo;

    private static UserDto ToDto(Core.Entities.User u) =>
        new(u.UserId, u.Username, u.Email, u.Balance, u.Role, u.IsBlocked, u.CreatedAt);

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        return user == null ? null : ToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync() =>
        (await _userRepo.GetAllUsersAsync()).Select(ToDto);

    public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return (false, "User not found.");

        var existing = await _userRepo.GetByUsernameAsync(request.Username);
        if (existing != null && existing.UserId != userId)
            return (false, "Username already taken.");

        user.Username = request.Username.Trim();
        user.Email = request.Email.Trim().ToLower();
        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return (true, "Profile updated.");
    }

    public async Task<(bool Success, string Message)> BlockUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return (false, "User not found.");
        user.IsBlocked = true;
        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return (true, "User blocked.");
    }

    public async Task<(bool Success, string Message)> UnblockUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return (false, "User not found.");
        user.IsBlocked = false;
        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        return (true, "User unblocked.");
    }

    public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return (false, "User not found.");
        await _userRepo.DeleteAsync(user);
        await _userRepo.SaveChangesAsync();
        return (true, "User deleted.");
    }
}
