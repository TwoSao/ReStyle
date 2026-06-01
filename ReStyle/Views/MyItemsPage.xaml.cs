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
}