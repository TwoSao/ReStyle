using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class HomeViewModel : ObservableObject, IRecipient<ItemsChangedMessage>
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _services;

    [ObservableProperty] private ObservableCollection<ItemDto> _items = new();
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isAuthenticated;

    public HomeViewModel(IAuthService authService, IServiceProvider services)
    {
        _authService = authService;
        _services = services;
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(ItemsChangedMessage message) =>
        MainThread.BeginInvokeOnMainThread(async () => await LoadItemsAsync());

    public async Task InitializeAsync()
    {
        IsAuthenticated = _authService.IsAuthenticated;
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        IsBusy = true;
        using var scope = _services.CreateScope();
        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();
        var items = await itemService.GetAvailableItemsAsync();
        Items = new ObservableCollection<ItemDto>(items);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsBusy = true;
        using var scope = _services.CreateScope();
        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();
        var items = await itemService.SearchItemsAsync(SearchQuery);
        Items = new ObservableCollection<ItemDto>(items);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task OpenItemAsync(ItemDto item) =>
        await NavigationService.GoToAsync("itemdetails", new Dictionary<string, object> { ["Item"] = item });

    [RelayCommand]
    private async Task GoToAddItemAsync()
    {
        if (!_authService.IsAuthenticated) { await NavigationService.GoToAsync("login"); return; }
        await NavigationService.GoToAsync("additem");
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        if (!_authService.IsAuthenticated) { await NavigationService.GoToAsync("login"); return; }
        await NavigationService.GoToAsync("//profile");
    }
}
