using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class AdminPage : ContentPage
{
    private readonly AdminViewModel _vm;

    public AdminPage(AdminViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
        UsersList.ItemsSource = _vm.Users;
    }

    private void OnUsersTab(object? sender, EventArgs e)
    {
        UsersList.ItemsSource = _vm.Users;
        SetTab(0);
    }

    private void OnTransactionsTab(object? sender, EventArgs e)
    {
        UsersList.ItemsSource = _vm.Transactions;
        SetTab(1);
    }

    private void SetTab(int active)
    {
        var primary   = (Color)Microsoft.Maui.Controls.Application.Current!.Resources["Primary"];
        var secondary = (Color)Microsoft.Maui.Controls.Application.Current.Resources["TextSecondary"];
        UsersTabBg.BackgroundColor = active == 0 ? primary : Colors.Transparent;
        TxTabBg.BackgroundColor    = active == 1 ? primary : Colors.Transparent;
    }
}
