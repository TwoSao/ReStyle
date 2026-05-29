using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _services;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private decimal _balance;
    [ObservableProperty] private string _role = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isAdmin;
    [ObservableProperty] private bool _isGuest;
    [ObservableProperty] private bool _isUser;

    public ProfileViewModel(IAuthService authService, IServiceProvider services)
    {
        _authService = authService;
        _services = services;
        _authService.AuthStateChanged += (_, _) => MainThread.BeginInvokeOnMainThread(Refresh);
    }

    public void Refresh()
    {
        IsUser = _authService.IsAuthenticated;
        IsGuest = _authService.IsGuest;
        IsAdmin = _authService.IsAdmin;

        if (!_authService.IsAuthenticated) return;

        var user = _authService.CurrentUser!;
        Username = user.Username;
        Email = user.Email;
        Balance = user.Balance;
        Role = user.Role.ToString();
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        if (_authService.CurrentUser is null) return;
        IsBusy = true;
        using var scope = _services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var (success, message) = await userService.UpdateProfileAsync(
            _authService.CurrentUser.UserId,
            new UpdateProfileRequest(Username, Email));
        IsBusy = false;
        ErrorMessage = success ? string.Empty : message;
        if (success)
        {
            _authService.CurrentUser.Username = Username;
            _authService.CurrentUser.Email = Email;
        }
    }

    [RelayCommand]
    private async Task GoToBalanceAsync() => await NavigationService.GoToAsync("//balance");

    [RelayCommand]
    private async Task GoToTransactionsAsync() => await NavigationService.GoToAsync("//transactions");

    [RelayCommand]
    private async Task GoToAdminAsync() => await NavigationService.GoToAsync("admin");

    [RelayCommand]
    private async Task GoToLoginAsync() => await NavigationService.GoToAsync("login");

    [RelayCommand]
    private async Task GoToRegisterAsync() => await NavigationService.GoToAsync("register");

    [RelayCommand]
    private void Logout() => _authService.Logout();
}
