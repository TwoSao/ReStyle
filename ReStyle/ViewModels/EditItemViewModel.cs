using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

[QueryProperty(nameof(ItemId), "ItemId")]
public partial class EditItemViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly IAuthService _authService;
    private readonly IImageService _imageService;

    [ObservableProperty] private int _itemId;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _price = string.Empty;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private string _size = string.Empty;
    [ObservableProperty] private string? _imagePath;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public List<string> Categories { get; } = ["Tops", "Bottoms", "Dresses", "Outerwear", "Shoes", "Accessories", "Other"];
    public List<string> Sizes { get; } = ["XS", "S", "M", "L", "XL", "XXL", "One Size"];

    public EditItemViewModel(IItemService itemService, IAuthService authService, IImageService imageService)
    {
        _itemService = itemService;
        _authService = authService;
        _imageService = imageService;
    }

    partial void OnItemIdChanged(int value) => _ = LoadItemAsync(value);

    private async Task LoadItemAsync(int id)
    {
        var item = await _itemService.GetItemByIdAsync(id);
        if (item == null) return;
        Title = item.Title;
        Description = item.Description;
        Price = item.Price.ToString();
        Category = item.Category;
        Size = item.Size;
        ImagePath = item.ImagePath;
    }

    [RelayCommand]
    private async Task PickImageAsync()
    {
        var result = await MediaPicker.PickPhotoAsync();
        if (result == null) return;
        var stream = await result.OpenReadAsync();
        ImagePath = await _imageService.SaveImageAsync(stream, result.FileName);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;
        if (!decimal.TryParse(Price, out var price)) { ErrorMessage = "Invalid price."; return; }

        IsBusy = true;
        var (success, message) = await _itemService.UpdateItemAsync(
            ItemId, _authService.CurrentUser!.UserId,
            new UpdateItemRequest(Title, Description, price, ImagePath, Category, Size),
            isAdmin: _authService.IsAdmin);
        IsBusy = false;

        if (!success) { ErrorMessage = message; return; }

        WeakReferenceMessenger.Default.Send(new ItemsChangedMessage());
        await NavigationService.GoBackAsync();
    }

    [RelayCommand]
    private async Task CancelAsync() => await NavigationService.GoBackAsync();
}
