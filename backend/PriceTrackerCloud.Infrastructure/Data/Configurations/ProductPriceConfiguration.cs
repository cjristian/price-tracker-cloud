using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Infrastructure.Data.Configurations;

public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
{
    public void Configure(EntityTypeBuilder<ProductPrice> builder)
    {
        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pp => pp.DateCollected)
            .IsRequired();

        builder.HasIndex(pp => new { pp.ProductId, pp.StoreId, pp.DateCollected });

        builder.HasData(SeedData.ProductPrices);
    }
}
