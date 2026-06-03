using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public RegisterViewModel(IAuthService authService) => _authService = authService;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;
        if (Password != ConfirmPassword) { ErrorMessage = "Paroolid ei ühti."; return; }

        IsBusy = true;
        var (success, message, _) = await _authService.RegisterAsync(new RegisterRequest(Username, Email, Password));
        IsBusy = false;

        if (!success) { ErrorMessage = message; return; }
        // AuthStateChanged fires → AppShell rebuilds tabs → navigates to //home
    }

    [RelayCommand]
    private async Task GoToLoginAsync() => await NavigationService.GoToAsync("login");
}
