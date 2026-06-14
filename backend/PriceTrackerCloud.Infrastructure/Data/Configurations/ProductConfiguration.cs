using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(p => p.Prices)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Alerts)
            .WithOne(a => a.Product)
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.Products);
    }
}
