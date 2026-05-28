using ReStyle.Application.DTOs;

namespace ReStyle.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<(bool Success, string Message)> BlockUserAsync(int userId);
    Task<(bool Success, string Message)> UnblockUserAsync(int userId);
    Task<(bool Success, string Message)> DeleteUserAsync(int userId);
}
