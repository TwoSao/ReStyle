using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly IAuthService _authService;

    [ObservableProperty] private ObservableCollection<ItemDto> _items = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isAuthenticated;

    public HomeViewModel(IItemService itemService, IAuthService authService)
    {
        _itemService = itemService;
        _authService = authService;
    }

    public async Task InitializeAsync()
    {
        IsAuthenticated = _authService.IsAuthenticated;
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        IsBusy = true;
        var items = await _itemService.GetAvailableItemsAsync();
        Items = new ObservableCollection<ItemDto>(items);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsBusy = true;
        var items = await _itemService.SearchItemsAsync(SearchQuery);
        Items = new ObservableCollection<ItemDto>(items);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task OpenItemAsync(ItemDto item) =>
        await NavigationService.GoToAsync("itemdetails", new Dictionary<string, object> { ["Item"] = item });

    [RelayCommand]
    private async Task GoToAddItemAsync()
    {
        if (!_authService.IsAuthenticated) { await NavigationService.GoToAsync("//login"); return; }
        await NavigationService.GoToAsync("additem");
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        if (!_authService.IsAuthenticated) { await NavigationService.GoToAsync("//login"); return; }
        await NavigationService.GoToAsync("//profile");
    }
}
