using ReStyle.Application.Interfaces;
using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class MyItemsPage : ContentPage
{
    private readonly MyItemsViewModel _vm;
    private readonly IAuthService _authService;

    public MyItemsPage(MyItemsViewModel vm, IAuthService authService)
    {
        InitializeComponent();
        _vm = vm;
        _authService = authService;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_authService.IsAuthenticated)
        {
            await Shell.Current.GoToAsync("login");
            return;
        }
        await _vm.InitializeAsync();
    }

    private void OnActiveTab(object? sender, EventArgs e) => SetTab(0);
    private void OnSoldTab(object? sender, EventArgs e) => SetTab(1);

    private void SetTab(int active)
    {
        var primary = (Color)Microsoft.Maui.Controls.Application.Current!.Resources["Primary"];
        var secondary = (Color)Microsoft.Maui.Controls.Application.Current.Resources["TextSecondary"];

        ActiveTabBg.BackgroundColor = active == 0 ? primary : Colors.Transparent;
        SoldTabBg.BackgroundColor = active == 1 ? primary : Colors.Transparent;

        ActiveList.IsVisible = active == 0;
        SoldList.IsVisible = active == 1;
    }
}
