using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Infrastructure.Data.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Website)
            .HasMaxLength(300);

        builder.HasMany(s => s.Prices)
            .WithOne(pp => pp.Store)
            .HasForeignKey(pp => pp.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.Stores);
    }
}
