using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Infrastructure.Data.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.TargetPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(a => a.IsActive)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();
    }
}
