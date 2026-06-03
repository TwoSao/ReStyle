using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReStyle.Core.Entities;

namespace ReStyle.Infrastructure.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.TransactionId);
        builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");

        builder.HasOne(t => t.Buyer)
               .WithMany(u => u.BuyerTransactions)
               .HasForeignKey(t => t.BuyerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Seller)
               .WithMany(u => u.SellerTransactions)
               .HasForeignKey(t => t.SellerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Item)
               .WithOne(i => i.Transaction)
               .HasForeignKey<Transaction>(t => t.ItemId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
