using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class MyItemsViewModel : ObservableObject, IRecipient<ItemsChangedMessage>
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _services;

    [ObservableProperty] private ObservableCollection<ItemDto> _items = new();
    [ObservableProperty] private bool _isBusy;

    public MyItemsViewModel(IAuthService authService, IServiceProvider services)
    {
        _authService = authService;
        _services = services;
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(ItemsChangedMessage message) =>
        MainThread.BeginInvokeOnMainThread(async () => await InitializeAsync());

    public async Task InitializeAsync()
    {
        if (_authService.CurrentUser == null) return;
        IsBusy = true;
        using var scope = _services.CreateScope();
        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();
        var items = await itemService.GetMyItemsAsync(_authService.CurrentUser.UserId);
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

        using var scope = _services.CreateScope();
        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();
        var (success, message) = await itemService.DeleteItemAsync(item.ItemId, _authService.CurrentUser!.UserId);
        if (success)
        {
            Items.Remove(item);
            WeakReferenceMessenger.Default.Send(new ItemsChangedMessage());
        }
        else await Shell.Current.DisplayAlert("Error", message, "OK");
    }

    [RelayCommand]
    private async Task AddItemAsync() => await NavigationService.GoToAsync("additem");
}
