using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _vm;
    private bool _showingPurchases = true;

    public TransactionsPage(TransactionsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
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
