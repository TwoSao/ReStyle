using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class AddItemPage : ContentPage
{
    public AddItemPage(AddItemViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
