using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class MyItemsPage : ContentPage
{
    private readonly MyItemsViewModel _vm;

    public MyItemsPage(MyItemsViewModel vm)
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
