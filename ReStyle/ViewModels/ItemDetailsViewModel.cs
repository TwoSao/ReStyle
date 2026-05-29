using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class ItemDetailsViewModel : ObservableObject
{
    private readonly IPurchaseService _purchaseService;
    private readonly IAuthService _authService;

    [ObservableProperty] private ItemDto? _item;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _canBuy;

    public ItemDetailsViewModel(IPurchaseService purchaseService, IAuthService authService)
    {
        _purchaseService = purchaseService;
        _authService = authService;
    }

    partial void OnItemChanged(ItemDto? value)
    {
        CanBuy = _authService.IsAuthenticated &&
                 value?.Status == Core.Enums.ItemStatus.Available &&
                 value?.UserId != _authService.CurrentUser?.UserId;
    }

    [RelayCommand]
    private async Task BuyAsync()
    {
        if (!_authService.IsAuthenticated)
        {
            await Shell.Current.GoToAsync("login");
            return;
        }
        if (Item == null || _authService.CurrentUser == null) return;
        IsBusy = true;
        var (success, message) = await _purchaseService.PurchaseItemAsync(_authService.CurrentUser.UserId, Item.ItemId);
        IsBusy = false;
        StatusMessage = message;
        if (success) CanBuy = false;
    }

    [RelayCommand]
    private async Task GoBackAsync() => await NavigationService.GoBackAsync();
}
