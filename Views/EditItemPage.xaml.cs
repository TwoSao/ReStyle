using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class EditItemPage : ContentPage
{
    public EditItemPage(EditItemViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
