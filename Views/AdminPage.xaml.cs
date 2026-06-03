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
    }
}
