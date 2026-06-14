using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Infrastructure.Data.Configurations;

// GUIDs fijos para que las migraciones sean deterministas
internal static class SeedData
{
    // Tiendas
    public static readonly Guid AmazonId      = new("a1a1a1a1-0000-0000-0000-000000000001");
    public static readonly Guid PcComponentesId = new("a1a1a1a1-0000-0000-0000-000000000002");
    public static readonly Guid MediaMarktId  = new("a1a1a1a1-0000-0000-0000-000000000003");
    public static readonly Guid ElCorteInglesId = new("a1a1a1a1-0000-0000-0000-000000000004");

    // Productos
    public static readonly Guid Ps5Id         = new("b2b2b2b2-0000-0000-0000-000000000001");
    public static readonly Guid IphoneId      = new("b2b2b2b2-0000-0000-0000-000000000002");
    public static readonly Guid MacbookId     = new("b2b2b2b2-0000-0000-0000-000000000003");

    public static readonly Store[] Stores =
    [
        new() { Id = AmazonId,       Name = "Amazon",           Website = "https://www.amazon.es" },
        new() { Id = PcComponentesId, Name = "PcComponentes",   Website = "https://www.pccomponentes.com" },
        new() { Id = MediaMarktId,   Name = "MediaMarkt",        Website = "https://www.mediamarkt.es" },
        new() { Id = ElCorteInglesId, Name = "El Corte Inglés",  Website = "https://www.elcorteingles.es" },
    ];

    public static readonly Product[] Products =
    [
        new() { Id = Ps5Id,    Name = "PlayStation 5",          Description = "Consola Sony PS5 Standard Edition", Category = "Videojuegos" },
        new() { Id = IphoneId, Name = "iPhone 15 Pro",          Description = "Apple iPhone 15 Pro 256GB",         Category = "Smartphones" },
        new() { Id = MacbookId, Name = "MacBook Pro 14 M3",     Description = "Apple MacBook Pro 14\" chip M3",   Category = "Portátiles" },
    ];

    // Histórico de precios para las gráficas (últimos 30 días aprox.)
    public static readonly ProductPrice[] ProductPrices = GeneratePrices();

    private static ProductPrice[] GeneratePrices()
    {
        var baseDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var prices = new List<ProductPrice>();
        var id = 1;

        // PS5 — bajada progresiva en Amazon
        decimal[] ps5Amazon = [549.99m, 539.99m, 529.99m, 519.99m, 509.99m, 499.99m];
        decimal[] ps5Mediamarkt = [559.99m, 549.99m, 549.99m, 539.99m, 529.99m, 519.99m];

        for (int i = 0; i < ps5Amazon.Length; i++)
        {
            prices.Add(new ProductPrice { Id = NewId(ref id), ProductId = Ps5Id, StoreId = AmazonId,     Price = ps5Amazon[i],     DateCollected = baseDate.AddDays(i * 5) });
            prices.Add(new ProductPrice { Id = NewId(ref id), ProductId = Ps5Id, StoreId = MediaMarktId, Price = ps5Mediamarkt[i], DateCollected = baseDate.AddDays(i * 5) });
        }

        // iPhone — PcComponentes vs El Corte Inglés
        decimal[] iphonePcc = [1229.00m, 1219.00m, 1199.00m, 1189.00m, 1179.00m, 1169.00m];
        decimal[] iphoneEci = [1249.00m, 1249.00m, 1229.00m, 1209.00m, 1199.00m, 1189.00m];

        for (int i = 0; i < iphonePcc.Length; i++)
        {
            prices.Add(new ProductPrice { Id = NewId(ref id), ProductId = IphoneId, StoreId = PcComponentesId, Price = iphonePcc[i], DateCollected = baseDate.AddDays(i * 5) });
            prices.Add(new ProductPrice { Id = NewId(ref id), ProductId = IphoneId, StoreId = ElCorteInglesId,  Price = iphoneEci[i], DateCollected = baseDate.AddDays(i * 5) });
        }

        // MacBook — Amazon vs PcComponentes
        decimal[] macAmazon = [2199.00m, 2149.00m, 2099.00m, 2049.00m, 1999.00m, 1979.00m];
        decimal[] macPcc    = [2249.00m, 2199.00m, 2149.00m, 2099.00m, 2049.00m, 1999.00m];

        for (int i = 0; i < macAmazon.Length; i++)
        {
            prices.Add(new ProductPrice { Id = NewId(ref id), ProductId = MacbookId, StoreId = AmazonId,       Price = macAmazon[i], DateCollected = baseDate.AddDays(i * 5) });
            prices.Add(new ProductPrice { Id = NewId(ref id), ProductId = MacbookId, StoreId = PcComponentesId, Price = macPcc[i],    DateCollected = baseDate.AddDays(i * 5) });
        }

        return [.. prices];
    }

    private static Guid NewId(ref int counter)
    {
        // Genera GUIDs deterministas para el seed
        return new Guid($"c3c3c3c3-0000-0000-0000-{counter++:D12}");
    }
}
