using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class BalancePage : ContentPage
{
    private readonly BalanceViewModel _vm;

    public BalancePage(BalanceViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Initialize();
    }

    private void OnQuickAmount(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string val)
            _vm.Amount = val;
    }
}
