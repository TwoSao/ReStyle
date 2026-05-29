using Microsoft.Extensions.DependencyInjection;
using ReStyle.Application.Interfaces;
using ReStyle.Views;

namespace ReStyle;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _services;
    private TabBar? _tabBar;

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
                await GoToAsync("//home");
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
        _tabBar = null;
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
        // If tabBar already exists, just sync the auth-only tabs — avoids full rebuild
        if (_tabBar != null)
        {
            SyncAuthTabs();
            return;
        }

        Items.Clear();
        Shell.SetNavBarIsVisible(this, true);

        _tabBar = new TabBar();
        _tabBar.Items.Add(MakeTab("Home",    "home.png",   "home",    () => _services.GetRequiredService<HomePage>()));
        _tabBar.Items.Add(MakeTab("Profile", "person.png", "profile", () => _services.GetRequiredService<ProfilePage>()));

        if (_authService.IsAuthenticated)
        {
            _tabBar.Items.Add(MakeTab("My Items",     "bag.png",     "myitems",      () => _services.GetRequiredService<MyItemsPage>()));
            _tabBar.Items.Add(MakeTab("Transactions", "receipt.png", "transactions", () => _services.GetRequiredService<TransactionsPage>()));
            _tabBar.Items.Add(MakeTab("Balance",      "wallet.png",  "balance",      () => _services.GetRequiredService<BalancePage>()));
        }

        Items.Add(_tabBar);
    }

    // Add or remove auth-only tabs without destroying the whole TabBar
    private void SyncAuthTabs()
    {
        if (_tabBar == null) return;

        var authRoutes = new[] { "myitems", "transactions", "balance" };
        var authTitles = new[] { "My Items", "Transactions", "Balance" };
        var authIcons  = new[] { "bag.png", "receipt.png", "wallet.png" };
        Func<Page>[] authFactories =
        [
            () => _services.GetRequiredService<MyItemsPage>(),
            () => _services.GetRequiredService<TransactionsPage>(),
            () => _services.GetRequiredService<BalancePage>()
        ];

        if (_authService.IsAuthenticated)
        {
            // Add missing auth tabs
            for (int i = 0; i < authRoutes.Length; i++)
            {
                var route = authRoutes[i];
                bool exists = _tabBar.Items.Any(t => t.Items.Any(c => c.Route == route));
                if (!exists)
                    _tabBar.Items.Add(MakeTab(authTitles[i], authIcons[i], route, authFactories[i]));
            }
        }
        else
        {
            // Remove auth tabs (guest mode)
            var toRemove = _tabBar.Items
                .Where(t => t.Items.Any(c => authRoutes.Contains(c.Route)))
                .ToList();
            foreach (var tab in toRemove)
                _tabBar.Items.Remove(tab);
        }
    }

    private static Tab MakeTab(string title, string icon, string route, Func<Page> factory)
    {
        var content = new ShellContent
        {
            Route = route,
            ContentTemplate = new DataTemplate(factory)
        };
        return new Tab
        {
            Title = title,
            Icon = icon,
            Items = { content }
        };
    }
}
