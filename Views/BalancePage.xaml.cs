using ReStyle.Application.Interfaces;
using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class BalancePage : ContentPage
{
    private readonly BalanceViewModel _vm;
    private readonly IAuthService _authService;

    public BalancePage(BalanceViewModel vm, IAuthService authService)
    {
        InitializeComponent();
        _vm = vm;
        _authService = authService;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_authService.IsAuthenticated)
        {
            Shell.Current.GoToAsync("login");
            return;
        }
        _vm.Initialize();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.Cleanup();
    }

    private void OnQuickAmount(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string val)
            _vm.Amount = val;
    }
}
