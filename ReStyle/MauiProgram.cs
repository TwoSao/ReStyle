using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReStyle.Application.Interfaces;
using ReStyle.Application.Services;
using ReStyle.Core.Interfaces;
using ReStyle.Infrastructure.Data;
using ReStyle.Infrastructure.Repositories;
using ReStyle.ViewModels;
using ReStyle.Views;

namespace ReStyle;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Database — Transient: each service gets its own DbContext, no EF identity-map cache between calls
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "restyle.db");
        builder.Services.AddDbContext<ReStyleDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"),
            ServiceLifetime.Transient, ServiceLifetime.Transient);

        // Repositories (Transient: each operation gets a fresh DbContext to avoid EF cache issues)
        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IItemRepository, ItemRepository>();
        builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
        builder.Services.AddTransient<IBalanceTopUpRepository, BalanceTopUpRepository>();

        // Services (Transient: fresh repos + DbContext per call)
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddTransient<IItemService, ItemService>();
        builder.Services.AddTransient<IPurchaseService, PurchaseService>();
        builder.Services.AddTransient<IBalanceService, BalanceService>();
        builder.Services.AddTransient<ITransactionService, TransactionService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddSingleton<IImageService, ImageService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        // Singleton: survives BuildTabs() rebuilds + WeakReferenceMessenger subscription stays alive
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddTransient<ItemDetailsViewModel>();
        builder.Services.AddTransient<AddItemViewModel>();
        builder.Services.AddTransient<EditItemViewModel>();
        builder.Services.AddSingleton<MyItemsViewModel>();
        builder.Services.AddSingleton<ProfileViewModel>();
        builder.Services.AddTransient<BalanceViewModel>();
        builder.Services.AddTransient<TransactionsViewModel>();
        builder.Services.AddTransient<AdminViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        // Singleton: same VM instance, OnAppearing fires correctly
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddTransient<ItemDetailsPage>();
        builder.Services.AddTransient<AddItemPage>();
        builder.Services.AddTransient<EditItemPage>();
        builder.Services.AddSingleton<MyItemsPage>();
        builder.Services.AddSingleton<ProfilePage>();
        builder.Services.AddTransient<BalancePage>();
        builder.Services.AddTransient<TransactionsPage>();
        builder.Services.AddTransient<AdminPage>();

        // Shell (singleton — manages tab visibility)
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        InitializeDatabase(app.Services);

        return app;
    }

    private static void InitializeDatabase(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReStyleDbContext>();
        db.Database.EnsureCreated();
    }
}
