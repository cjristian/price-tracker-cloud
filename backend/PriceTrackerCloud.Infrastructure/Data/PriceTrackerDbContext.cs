using Microsoft.EntityFrameworkCore;
using PriceTrackerCloud.Domain.Entities;
using PriceTrackerCloud.Infrastructure.Data.Configurations;

namespace PriceTrackerCloud.Infrastructure.Data;

public class PriceTrackerDbContext : DbContext
{
    public PriceTrackerDbContext(DbContextOptions<PriceTrackerDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<ProductPrice> ProductPrices => Set<ProductPrice>();
    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas las IEntityTypeConfiguration del mismo ensamblado automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PriceTrackerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
