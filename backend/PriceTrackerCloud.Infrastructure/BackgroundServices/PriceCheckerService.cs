using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Infrastructure.BackgroundServices;

public class PriceCheckerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PriceCheckerService> _logger;
    private readonly Random _random = new();

    public PriceCheckerService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<PriceCheckerService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("PriceCheckerService: iniciando revisión de precios");
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                await RunIterationAsync(uow, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en PriceCheckerService durante la iteración");
            }

            var intervalMinutes = int.TryParse(_configuration["PriceChecker:IntervalMinutes"], out var m) ? m : 60;
            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
        }
    }

    internal async Task RunIterationAsync(IUnitOfWork uow, CancellationToken ct = default)
    {
        var allPrices = (await uow.Prices.GetAllAsync()).ToList();

        var latestByProductStore = allPrices
            .GroupBy(p => (p.ProductId, p.StoreId))
            .Select(g => g.OrderByDescending(p => p.DateCollected).First())
            .ToList();

        if (latestByProductStore.Count == 0)
        {
            _logger.LogInformation("PriceCheckerService: no hay precios en BD, omitiendo iteración");
            return;
        }

        var newPrices = new List<ProductPrice>();
        foreach (var latest in latestByProductStore)
        {
            var newPrice = new ProductPrice
            {
                Id            = Guid.NewGuid(),
                ProductId     = latest.ProductId,
                StoreId       = latest.StoreId,
                Price         = SimulateNewPrice(latest.Price),
                DateCollected = DateTime.UtcNow
            };
            await uow.Prices.AddAsync(newPrice);
            newPrices.Add(newPrice);
        }

        await uow.SaveChangesAsync(ct);
        _logger.LogInformation("Insertados {Count} nuevos registros de precio", newPrices.Count);

        var activeAlerts = (await uow.Alerts.GetActiveAlertsAsync()).ToList();
        if (activeAlerts.Count == 0)
        {
            _logger.LogInformation("Revisión completada: ninguna alerta activa");
            return;
        }

        var anyTriggered = false;
        foreach (var alert in activeAlerts)
        {
            var pricesForProduct = newPrices.Where(p => p.ProductId == alert.ProductId).ToList();
            if (pricesForProduct.Count == 0) continue;

            var minPrice = pricesForProduct.Min(p => p.Price);
            if (minPrice <= alert.TargetPrice)
            {
                alert.IsActive = false;
                uow.Alerts.Update(alert);
                _logger.LogInformation(
                    "Alerta {AlertId} disparada — ProductId {ProductId}, precio {Price} ≤ objetivo {Target}",
                    alert.Id, alert.ProductId, minPrice, alert.TargetPrice);
                anyTriggered = true;
            }
        }

        if (anyTriggered)
            await uow.SaveChangesAsync(ct);
        else
            _logger.LogInformation("Revisión completada: ninguna alerta disparada");
    }

    protected virtual decimal SimulateNewPrice(decimal currentPrice)
    {
        var factor = 0.90m + (decimal)_random.NextDouble() * 0.20m;
        return Math.Round(currentPrice * factor, 2);
    }
}
