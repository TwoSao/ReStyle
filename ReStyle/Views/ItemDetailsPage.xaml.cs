using ReStyle.ViewModels;

namespace ReStyle.Views;

public partial class ItemDetailsPage : ContentPage
{
    public ItemDetailsPage(ItemDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
