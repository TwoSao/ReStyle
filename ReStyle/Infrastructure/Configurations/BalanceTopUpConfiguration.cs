using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReStyle.Core.Entities;

namespace ReStyle.Infrastructure.Configurations;

public class BalanceTopUpConfiguration : IEntityTypeConfiguration<BalanceTopUp>
{
    public void Configure(EntityTypeBuilder<BalanceTopUp> builder)
    {
        builder.HasKey(b => b.TopUpId);
        builder.Property(b => b.Amount).HasColumnType("decimal(18,2)");
        builder.Property(b => b.CardNumberMasked).HasMaxLength(20);

        builder.HasOne(b => b.User)
               .WithMany(u => u.BalanceTopUps)
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
