using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class BalanceViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IBalanceService _balanceService;

    [ObservableProperty] private decimal _balance;
    [ObservableProperty] private string _amount = string.Empty;
    [ObservableProperty] private string _cardNumber = string.Empty;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public BalanceViewModel(IAuthService authService, IBalanceService balanceService)
    {
        _authService = authService;
        _balanceService = balanceService;
    }

    public void Initialize() 
    {
        Balance = _authService.CurrentUser?.Balance ?? 0;
        _authService.AuthStateChanged += OnAuthStateChanged;
    }

    public void Cleanup() => _authService.AuthStateChanged -= OnAuthStateChanged;

    private void OnAuthStateChanged(object? sender, EventArgs e) =>
        MainThread.BeginInvokeOnMainThread(() => Balance = _authService.CurrentUser?.Balance ?? 0);

    [RelayCommand]
    private async Task TopUpAsync()
    {
        if (!decimal.TryParse(Amount, out var amt)) { Message = "Invalid amount."; return; }

        IsBusy = true;
        var (success, msg) = await _balanceService.TopUpAsync(
            _authService.CurrentUser!.UserId,
            new TopUpRequest(amt, CardNumber.Replace(" ", "")));
        IsBusy = false;
        Message = msg;

        if (success)
        {
            _authService.CurrentUser!.Balance += amt;
            Balance = _authService.CurrentUser.Balance;
            Amount = string.Empty;
            CardNumber = string.Empty;
            _authService.NotifyBalanceChanged();
        }
    }

    [RelayCommand]
    private async Task GoBackAsync() => await NavigationService.GoBackAsync();
}
