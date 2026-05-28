using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private decimal _balance;
    [ObservableProperty] private string _role = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isAdmin;

    public ProfileViewModel(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    public void Initialize()
    {
        var user = _authService.CurrentUser;
        if (user == null) return;
        Username = user.Username;
        Email = user.Email;
        Balance = user.Balance;
        Role = user.Role.ToString();
        IsAdmin = _authService.IsAdmin;
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        IsBusy = true;
        var (success, message) = await _userService.UpdateProfileAsync(
            _authService.CurrentUser!.UserId,
            new UpdateProfileRequest(Username, Email));
        IsBusy = false;
        ErrorMessage = success ? string.Empty : message;
        if (success)
        {
            _authService.CurrentUser!.Username = Username;
            _authService.CurrentUser!.Email = Email;
        }
    }

    [RelayCommand]
    private async Task GoToBalanceAsync() => await NavigationService.GoToAsync("//balance");

    [RelayCommand]
    private async Task GoToTransactionsAsync() => await NavigationService.GoToAsync("//transactions");

    [RelayCommand]
    private async Task GoToAdminAsync() => await NavigationService.GoToAsync("//admin");

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        Shell.Current.GoToAsync("//login");
    }
}
