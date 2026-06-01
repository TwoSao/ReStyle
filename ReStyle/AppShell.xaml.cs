using Microsoft.Extensions.DependencyInjection;
using ReStyle.Application.Interfaces;
using ReStyle.Views;

namespace ReStyle;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _services;

    public AppShell(IAuthService authService, IServiceProvider services)
    {
        InitializeComponent();
        _authService = authService;
        _services = services;

        RegisterRoutes();
        BuildLoginStartup();

        _authService.AuthStateChanged += OnAuthStateChanged;
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("itemdetails", typeof(ItemDetailsPage));
        Routing.RegisterRoute("additem", typeof(AddItemPage));
        Routing.RegisterRoute("edititem", typeof(EditItemPage));
        Routing.RegisterRoute("admin", typeof(AdminPage));
    }

    private void OnAuthStateChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (_authService.IsAuthenticated || _authService.IsGuest)
            {
                BuildTabs();
                // Go to profile so user immediately sees their account data
                var target = _authService.IsAuthenticated ? "//profile" : "//home";
                await GoToAsync(target);
            }
            else
            {
                BuildLoginStartup();
            }
        });
    }

    private void BuildLoginStartup()
    {
        Items.Clear();
        var content = new ShellContent
        {
            Route = "loginstart",
            ContentTemplate = new DataTemplate(() => _services.GetRequiredService<LoginPage>())
        };
        Shell.SetNavBarIsVisible(content, false);
        Items.Add(content);
    }

    private void BuildTabs()
    {
        Items.Clear();
        Shell.SetNavBarIsVisible(this, true);

        var tabBar = new TabBar();
        tabBar.Items.Add(MakeTab("Home",    "home.png",    "home",    () => _services.GetRequiredService<HomePage>()));
        tabBar.Items.Add(MakeTab("Profile", "person.png",  "profile", () => _services.GetRequiredService<ProfilePage>()));

        if (_authService.IsAuthenticated)
        {
            tabBar.Items.Add(MakeTab("My Items",     "bag.png",     "myitems",      () => _services.GetRequiredService<MyItemsPage>()));
            tabBar.Items.Add(MakeTab("Transactions", "receipt.png", "transactions", () => _services.GetRequiredService<TransactionsPage>()));
            tabBar.Items.Add(MakeTab("Balance",      "wallet.png",  "balance",      () => _services.GetRequiredService<BalancePage>()));
        }

        Items.Add(tabBar);
    }

    private static Tab MakeTab(string title, string icon, string route, Func<Page> factory)
    {
        var content = new ShellContent
        {
            Route = route,
            ContentTemplate = new DataTemplate(factory)
        };
        return new Tab { Title = title, Icon = icon, Items = { content } };
    }
}
