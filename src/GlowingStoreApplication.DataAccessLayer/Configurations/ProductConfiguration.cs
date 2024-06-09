using GlowingStoreApplication.DataAccessLayer.Configurations.Common;
using GlowingStoreApplication.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowingStoreApplication.DataAccessLayer.Configurations;

public class ProductConfiguration : DeletableEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(256).IsRequired();
        builder.Property(p => p.Description).HasColumnType("NVARCHAR(MAX)").IsRequired();

        builder.Property(p => p.Quantity).IsRequired();
        builder.Property(p => p.Price).HasPrecision(8, 2).IsRequired();
        builder.Property(p => p.DiscountPercentage).IsRequired(false);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .IsRequired();

        builder.HasIndex(p => new { p.CategoryId, p.Name, p.Price })
            .HasDatabaseName("IX_Product")
            .IsUnique();

        builder.ToTable("Products");
        base.Configure(builder);
    }
}