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


    }
}
