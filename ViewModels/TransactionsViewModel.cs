using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class TransactionsViewModel : ObservableObject
{
    private readonly ITransactionService _transactionService;
    private readonly IAuthService _authService;

    [ObservableProperty] private ObservableCollection<TransactionDto> _purchases = new();
    [ObservableProperty] private ObservableCollection<TransactionDto> _sales = new();
    [ObservableProperty] private bool _isBusy;

    public TransactionsViewModel(ITransactionService transactionService, IAuthService authService)
    {
        _transactionService = transactionService;
        _authService = authService;
    }

    public async Task InitializeAsync()
    {
        if (_authService.CurrentUser == null) return;
        IsBusy = true;
        var userId = _authService.CurrentUser.UserId;
        var purchases = await _transactionService.GetMyPurchasesAsync(userId);
        var sales = await _transactionService.GetMySalesAsync(userId);
        Purchases = new ObservableCollection<TransactionDto>(purchases);
        Sales = new ObservableCollection<TransactionDto>(sales);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task GoBackAsync() => await NavigationService.GoBackAsync();
}
