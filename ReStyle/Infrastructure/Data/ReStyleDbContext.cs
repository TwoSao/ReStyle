using Microsoft.EntityFrameworkCore;
using ReStyle.Core.Entities;
using ReStyle.Core.Enums;
using ReStyle.Infrastructure.Configurations;

namespace ReStyle.Infrastructure.Data;

public class ReStyleDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<BalanceTopUp> BalanceTopUps => Set<BalanceTopUp>();

    public ReStyleDbContext(DbContextOptions<ReStyleDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ItemConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new BalanceTopUpConfiguration());

        // Seed admin user
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = 1,
            Username = "admin",
            Email = "admin@restyle.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Balance = 0,
            Role = UserRole.Admin,
            IsBlocked = false,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
