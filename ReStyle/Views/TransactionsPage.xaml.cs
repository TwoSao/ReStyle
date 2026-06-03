using ReStyle.Application.Interfaces;
using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _vm;
    private readonly IAuthService _authService;
    public TransactionsPage(TransactionsViewModel vm, IAuthService authService)
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
        TransactionsList.ItemsSource = _vm.Purchases;
    }

    private void OnPurchasesTab(object? sender, EventArgs e)
    {
        TransactionsList.ItemsSource = _vm.Purchases;
        SetTab(0);
    }

    private void OnSalesTab(object? sender, EventArgs e)
    {
        TransactionsList.ItemsSource = _vm.Sales;
        SetTab(1);
    }

    private void SetTab(int active)
    {
        var primary   = (Color)Microsoft.Maui.Controls.Application.Current!.Resources["Primary"];
        var secondary = (Color)Microsoft.Maui.Controls.Application.Current.Resources["TextSecondary"];
        PurchasesTabBg.BackgroundColor = active == 0 ? primary : Colors.Transparent;
        SalesTabBg.BackgroundColor     = active == 1 ? primary : Colors.Transparent;
        PurchasesBtn.TextColor = active == 0 ? Colors.White : secondary;
        SalesBtn.TextColor     = active == 1 ? Colors.White : secondary;
    }
}
