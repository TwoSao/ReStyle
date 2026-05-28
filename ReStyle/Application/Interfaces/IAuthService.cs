using ReStyle.Application.DTOs;
using ReStyle.Core.Entities;

namespace ReStyle.Application.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterRequest request);
    Task<(bool Success, string Message, User? User)> LoginAsync(LoginRequest request);
    void Logout();
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
}
