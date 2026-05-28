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

        // Database
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "restyle.db");
        builder.Services.AddDbContext<ReStyleDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<IBalanceTopUpRepository, BalanceTopUpRepository>();

        // Services (Singleton for auth session state)
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddScoped<IItemService, ItemService>();
        builder.Services.AddScoped<IPurchaseService, PurchaseService>();
        builder.Services.AddScoped<IBalanceService, BalanceService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddSingleton<IImageService, ImageService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ItemDetailsViewModel>();
        builder.Services.AddTransient<AddItemViewModel>();
        builder.Services.AddTransient<EditItemViewModel>();
        builder.Services.AddTransient<MyItemsViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<BalanceViewModel>();
        builder.Services.AddTransient<TransactionsViewModel>();
        builder.Services.AddTransient<AdminViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ItemDetailsPage>();
        builder.Services.AddTransient<AddItemPage>();
        builder.Services.AddTransient<EditItemPage>();
        builder.Services.AddTransient<MyItemsPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<BalancePage>();
        builder.Services.AddTransient<TransactionsPage>();
        builder.Services.AddTransient<AdminPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Initialize DB (create + migrate + seed)
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
