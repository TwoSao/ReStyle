using ReStyle.Views;

namespace ReStyle;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register modal/detail routes
        Routing.RegisterRoute("itemdetails", typeof(ItemDetailsPage));
        Routing.RegisterRoute("additem", typeof(AddItemPage));
        Routing.RegisterRoute("edititem", typeof(EditItemPage));
        Routing.RegisterRoute("admin", typeof(AdminPage));
    }
}
