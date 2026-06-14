using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Application.Interfaces.Repositories;
using PriceTrackerCloud.Domain.Entities;
using PriceTrackerCloud.Infrastructure.BackgroundServices;

namespace PriceTrackerCloud.Tests.BackgroundServices;

public class PriceCheckerServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IPriceRepository> _priceRepoMock = new();
    private readonly Mock<IAlertRepository> _alertRepoMock = new();
    private readonly IConfiguration _config;

    public PriceCheckerServiceTests()
    {
        _uowMock.Setup(u => u.Prices).Returns(_priceRepoMock.Object);
        _uowMock.Setup(u => u.Alerts).Returns(_alertRepoMock.Object);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["PriceChecker:IntervalMinutes"] = "60"
            })
            .Build();
    }

    private TestablePriceCheckerService CreateSut(decimal fixedSimulatedPrice) =>
        new(new Mock<IServiceScopeFactory>().Object,
            _config,
            new Mock<ILogger<PriceCheckerService>>().Object,
            fixedSimulatedPrice);

    [Fact]
    public async Task RunIterationAsync_WhenPriceDropsBelowTarget_DeactivatesAlert()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var storeId   = Guid.NewGuid();

        var existingPrice = new ProductPrice
        {
            Id            = Guid.NewGuid(),
            ProductId     = productId,
            StoreId       = storeId,
            Price         = 100m,
            DateCollected = DateTime.UtcNow.AddHours(-1)
        };

        var alert = new Alert
        {
            Id          = Guid.NewGuid(),
            ProductId   = productId,
            TargetPrice = 95m,
            IsActive    = true
        };

        _priceRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { existingPrice });
        _alertRepoMock.Setup(r => r.GetActiveAlertsAsync()).ReturnsAsync(new[] { alert });

        // fixedSimulatedPrice=90 → 90 < 95 → debe disparar la alerta
        var sut = CreateSut(fixedSimulatedPrice: 90m);

        // Act
        await sut.RunAsync(_uowMock.Object);

        // Assert
        alert.IsActive.Should().BeFalse();
        // SaveChangesAsync: una vez para precios nuevos + otra para alertas disparadas
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RunIterationAsync_WhenPriceAboveTarget_AlertRemainsActive()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var storeId   = Guid.NewGuid();

        var existingPrice = new ProductPrice
        {
            Id            = Guid.NewGuid(),
            ProductId     = productId,
            StoreId       = storeId,
            Price         = 100m,
            DateCollected = DateTime.UtcNow.AddHours(-1)
        };

        var alert = new Alert
        {
            Id          = Guid.NewGuid(),
            ProductId   = productId,
            TargetPrice = 95m,
            IsActive    = true
        };

        _priceRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { existingPrice });
        _alertRepoMock.Setup(r => r.GetActiveAlertsAsync()).ReturnsAsync(new[] { alert });

        // fixedSimulatedPrice=110 → 110 > 95 → no debe disparar la alerta
        var sut = CreateSut(fixedSimulatedPrice: 110m);

        // Act
        await sut.RunAsync(_uowMock.Object);

        // Assert
        alert.IsActive.Should().BeTrue();
        // SaveChangesAsync solo una vez (para los precios nuevos), nunca para alertas
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // Subclase de prueba: fija el precio simulado y expone RunIterationAsync (internal via InternalsVisibleTo)
    private sealed class TestablePriceCheckerService : PriceCheckerService
    {
        private readonly decimal _fixedPrice;

        public TestablePriceCheckerService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<PriceCheckerService> logger,
            decimal fixedPrice)
            : base(scopeFactory, configuration, logger)
        {
            _fixedPrice = fixedPrice;
        }

        protected override decimal SimulateNewPrice(decimal currentPrice) => _fixedPrice;

        public Task RunAsync(IUnitOfWork uow) => RunIterationAsync(uow);
    }
}
