using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly IItemService _itemService;
    private readonly ITransactionService _transactionService;

    [ObservableProperty] private ObservableCollection<UserDto> _users = new();
    [ObservableProperty] private ObservableCollection<TransactionDto> _transactions = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private int _selectedTab;

    public AdminViewModel(IUserService userService, IItemService itemService, ITransactionService transactionService)
    {
        _userService = userService;
        _itemService = itemService;
        _transactionService = transactionService;
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        var users = await _userService.GetAllUsersAsync();
        var transactions = await _transactionService.GetAllTransactionsAsync();
        Users = new ObservableCollection<UserDto>(users);
        Transactions = new ObservableCollection<TransactionDto>(transactions);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task BlockUserAsync(UserDto user)
    {
        var (success, message) = user.IsBlocked
            ? await _userService.UnblockUserAsync(user.UserId)
            : await _userService.BlockUserAsync(user.UserId);

        if (success) await InitializeAsync();
        else await Shell.Current.DisplayAlert("Error", message, "OK");
    }

    [RelayCommand]
    private async Task DeleteUserAsync(UserDto user)
    {
        var confirm = await Shell.Current.DisplayAlert("Delete User", $"Delete user '{user.Username}'?", "Yes", "No");
        if (!confirm) return;
        var (success, message) = await _userService.DeleteUserAsync(user.UserId);
        if (success) Users.Remove(user);
        else await Shell.Current.DisplayAlert("Error", message, "OK");
    }

    [RelayCommand]
    private async Task GoBackAsync() => await NavigationService.GoBackAsync();
}
