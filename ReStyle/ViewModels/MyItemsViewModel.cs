using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class MyItemsViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly IAuthService _authService;

    [ObservableProperty] private ObservableCollection<ItemDto> _items = new();
    [ObservableProperty] private bool _isBusy;

    public MyItemsViewModel(IItemService itemService, IAuthService authService)
    {
        _itemService = itemService;
        _authService = authService;
    }

    public async Task InitializeAsync()
    {
        if (_authService.CurrentUser == null) return;
        IsBusy = true;
        var items = await _itemService.GetMyItemsAsync(_authService.CurrentUser.UserId);
        Items = new ObservableCollection<ItemDto>(items);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task EditItemAsync(ItemDto item) =>
        await NavigationService.GoToAsync("edititem", new Dictionary<string, object> { ["ItemId"] = item.ItemId });

    [RelayCommand]
    private async Task DeleteItemAsync(ItemDto item)
    {
        var confirm = await Shell.Current.DisplayAlert("Delete", $"Delete '{item.Title}'?", "Yes", "No");
        if (!confirm) return;

        var (success, message) = await _itemService.DeleteItemAsync(item.ItemId, _authService.CurrentUser!.UserId);
        if (success) Items.Remove(item);
        else await Shell.Current.DisplayAlert("Error", message, "OK");
    }

    [RelayCommand]
    private async Task AddItemAsync() => await NavigationService.GoToAsync("additem");
}
