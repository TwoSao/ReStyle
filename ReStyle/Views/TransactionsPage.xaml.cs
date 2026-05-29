using ReStyle.Application.Interfaces;
using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _vm;
    private readonly IAuthService _authService;
    private bool _showingPurchases = true;

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

    private void OnPurchasesTab(object sender, EventArgs e)
    {
        _showingPurchases = true;
        TransactionsList.ItemsSource = _vm.Purchases;
    }

    private void OnSalesTab(object sender, EventArgs e)
    {
        _showingPurchases = false;
        TransactionsList.ItemsSource = _vm.Sales;
    }
}
