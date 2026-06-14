using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PriceTrackerCloud.Infrastructure.Data;

// Permite ejecutar "dotnet ef migrations" sin necesitar la app corriendo.
// La connection string aquí es solo para diseño/migraciones; en runtime se usa appsettings.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PriceTrackerDbContext>
{
    public PriceTrackerDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PriceTrackerDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=pricetracker;Username=postgres;Password=postgres")
            .Options;

        return new PriceTrackerDbContext(options);
    }
}
