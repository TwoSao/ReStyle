using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public LoginViewModel(IAuthService authService) => _authService = authService;

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Palun täida kõik väljad.";
            return;
        }

        IsBusy = true;
        var (success, message, _) = await _authService.LoginAsync(new LoginRequest(Email, Password));
        IsBusy = false;

        if (!success) { ErrorMessage = message; return; }

        // AuthStateChanged fires → AppShell rebuilds tabs → navigates to //home
    }

    [RelayCommand]
    private async Task GoToRegisterAsync() => await NavigationService.GoToAsync("register");

    [RelayCommand]
    private void ContinueAsGuest()
    {
        // Sets guest state → AuthStateChanged fires → AppShell rebuilds tabs → navigates to //home
        _authService.ContinueAsGuest();
    }
}
