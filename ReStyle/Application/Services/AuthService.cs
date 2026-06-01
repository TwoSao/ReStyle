using Microsoft.Extensions.DependencyInjection;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Core.Entities;
using ReStyle.Core.Enums;
using ReStyle.Core.Interfaces;

namespace ReStyle.Application.Services;

public class AuthService : IAuthService
{
    private readonly IServiceProvider _serviceProvider;
    private bool _isGuest;

    public User? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;
    // Guest only when explicitly set AND not authenticated — authenticated state always wins
    public bool IsGuest => !IsAuthenticated && _isGuest;
    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;

    public event EventHandler? AuthStateChanged;

    public AuthService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    private IUserRepository GetRepo()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IUserRepository>();
    }

    public void ContinueAsGuest()
    {
        // Guard: never allow guest mode to overwrite an authenticated session
        if (IsAuthenticated) return;
        _isGuest = true;
        AuthStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return (false, "All fields are required.", null);

        if (request.Password.Length < 6)
            return (false, "Password must be at least 6 characters.", null);

        var repo = GetRepo();

        if (await repo.GetByEmailAsync(request.Email) != null)
            return (false, "Email already in use.", null);

        if (await repo.GetByUsernameAsync(request.Username) != null)
            return (false, "Username already taken.", null);

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User
        };

        await repo.AddAsync(user);
        await repo.SaveChangesAsync();
        CurrentUser = user;
        _isGuest = false;
        AuthStateChanged?.Invoke(this, EventArgs.Empty);
        return (true, "Registration successful.", user);
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(LoginRequest request)
    {
        var repo = GetRepo();
        var user = await repo.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return (false, "Invalid email or password.", null);

        if (user.IsBlocked)
            return (false, "Your account has been blocked.", null);

        CurrentUser = user;
        _isGuest = false;
        AuthStateChanged?.Invoke(this, EventArgs.Empty);
        return (true, "Login successful.", user);
    }

    public void Logout()
    {
        CurrentUser = null;
        _isGuest = false;
        AuthStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void NotifyBalanceChanged() => AuthStateChanged?.Invoke(this, EventArgs.Empty);
}
