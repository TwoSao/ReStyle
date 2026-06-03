using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class ItemDetailsViewModel : ObservableObject
{
    private readonly IPurchaseService _purchaseService;
    private readonly IAuthService _authService;
    private readonly IServiceProvider _services;

    [ObservableProperty] private ItemDto? _item;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _canBuy;
    [ObservableProperty] private bool _isAdmin;
    public bool ShowNotAvailable => !CanBuy && !IsAdmin;

    public ItemDetailsViewModel(IPurchaseService purchaseService, IAuthService authService, IServiceProvider services)
    {
        _purchaseService = purchaseService;
        _authService = authService;
        _services = services;
        IsAdmin = _authService.IsAdmin;
    }

    partial void OnItemChanged(ItemDto? value)
    {
        CanBuy = _authService.IsAuthenticated &&
                 !_authService.IsAdmin &&
                 value?.Status == Core.Enums.ItemStatus.Available &&
                 value?.UserId != _authService.CurrentUser?.UserId;
        OnPropertyChanged(nameof(ShowNotAvailable));
    }

    partial void OnCanBuyChanged(bool value) => OnPropertyChanged(nameof(ShowNotAvailable));

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
    private async Task EditAsync()
    {
        if (Item == null) return;
        await NavigationService.GoToAsync("edititem", new Dictionary<string, object> { ["ItemId"] = Item.ItemId });
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Item == null) return;
        var confirm = await Shell.Current.DisplayAlert("Delete", $"Delete '{Item.Title}'?", "Yes", "No");
        if (!confirm) return;

        using var scope = _services.CreateScope();
        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();
        var (success, message) = await itemService.DeleteItemAsync(Item.ItemId, _authService.CurrentUser!.UserId, isAdmin: true);
        if (success)
        {
            WeakReferenceMessenger.Default.Send(new ItemsChangedMessage());
            await NavigationService.GoBackAsync();
        }
        else await Shell.Current.DisplayAlert("Error", message, "OK");
    }

    [RelayCommand]
    private async Task GoBackAsync() => await NavigationService.GoBackAsync();
}
