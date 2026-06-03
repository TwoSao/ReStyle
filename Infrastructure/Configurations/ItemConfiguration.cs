using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReStyle.Core.Entities;

namespace ReStyle.Infrastructure.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(i => i.ItemId);
        builder.Property(i => i.Title).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Description).HasMaxLength(1000);
        builder.Property(i => i.Price).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Status).HasConversion<int>();
        builder.Property(i => i.Category).HasMaxLength(50);
        builder.Property(i => i.Size).HasMaxLength(20);

        builder.HasOne(i => i.User)
               .WithMany(u => u.Items)
               .HasForeignKey(i => i.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
