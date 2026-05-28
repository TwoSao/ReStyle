using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;
using ReStyle.Helpers;

namespace ReStyle.ViewModels;

public partial class AddItemViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly IAuthService _authService;
    private readonly IImageService _imageService;

    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _price = string.Empty;
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private string _size = string.Empty;
    [ObservableProperty] private string? _imagePath;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public List<string> Categories { get; } = new() { "Tops", "Bottoms", "Dresses", "Outerwear", "Shoes", "Accessories", "Other" };
    public List<string> Sizes { get; } = new() { "XS", "S", "M", "L", "XL", "XXL", "One Size" };

    public AddItemViewModel(IItemService itemService, IAuthService authService, IImageService imageService)
    {
        _itemService = itemService;
        _authService = authService;
        _imageService = imageService;
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
        var (success, message, _) = await _itemService.CreateItemAsync(
            _authService.CurrentUser!.UserId,
            new CreateItemRequest(Title, Description, price, ImagePath, Category, Size));
        IsBusy = false;

        if (!success) { ErrorMessage = message; return; }
        await NavigationService.GoToAsync("//myitems");
    }

    [RelayCommand]
    private async Task CancelAsync() => await NavigationService.GoBackAsync();
}
