# Fase 4 — PriceCheckerService Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implementar un `BackgroundService` que cada N minutos simule precios nuevos, los persista en BD, compare contra alertas activas y desactive las que se hayan disparado, logueando todo con Serilog.

**Architecture:** `PriceCheckerService : BackgroundService` vive en Infrastructure. Su loop llama a `RunIterationAsync(IUnitOfWork)` que contiene toda la lógica real. Usa `IServiceScopeFactory` para obtener un scope fresco (y un `IUnitOfWork` scoped) en cada iteración. `SimulateNewPrice` es un método `protected virtual` para permitir subclases en tests que devuelvan un precio fijo predecible.

**Tech Stack:** .NET 9 · BackgroundService · EF Core 9 / IUnitOfWork · Serilog (ILogger) · xUnit + Moq + FluentAssertions

---

## File Map

| Acción | Archivo |
|--------|---------|
| Modify | `backend/PriceTrackerCloud.API/appsettings.json` |
| Modify | `backend/PriceTrackerCloud.Infrastructure/PriceTrackerCloud.Infrastructure.csproj` |
| Create | `backend/PriceTrackerCloud.Infrastructure/InternalsVisibleTo.cs` |
| Create | `backend/PriceTrackerCloud.Infrastructure/BackgroundServices/PriceCheckerService.cs` |
| Modify | `backend/PriceTrackerCloud.Infrastructure/DependencyInjection.cs` |
| Create | `backend/PriceTrackerCloud.Tests/BackgroundServices/PriceCheckerServiceTests.cs` |

---

## Task 1: Configuración y paquete base

**Files:**
- Modify: `backend/PriceTrackerCloud.API/appsettings.json`
- Modify: `backend/PriceTrackerCloud.Infrastructure/PriceTrackerCloud.Infrastructure.csproj`

- [ ] **Step 1: Añadir sección `PriceChecker` a `appsettings.json`**

Abrir `backend/PriceTrackerCloud.API/appsettings.json` y añadir la sección al final del objeto JSON:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=pricetracker;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "SecretKey": "REPLACE_WITH_ENV_VAR",
    "Issuer": "PriceTrackerCloud",
    "Audience": "PriceTrackerCloudUsers",
    "ExpirationMinutes": 60
  },
  "PriceChecker": {
    "IntervalMinutes": 60
  }
}
```

- [ ] **Step 2: Añadir `Microsoft.Extensions.Hosting.Abstractions` a Infrastructure**

`BackgroundService` vive en este paquete. Infrastructure es una class library (`Microsoft.NET.Sdk`), así que no lo tiene implícitamente.

Ejecutar desde `backend/`:
```bash
dotnet add PriceTrackerCloud.Infrastructure package Microsoft.Extensions.Hosting.Abstractions --version 9.0.5
```

Resultado esperado: `PackageReference` añadido en `PriceTrackerCloud.Infrastructure.csproj`.

- [ ] **Step 3: Verificar que el proyecto compila**

```bash
dotnet build PriceTrackerCloud.sln
```

Resultado esperado:
```
Build succeeded.
0 Error(s)
```

- [ ] **Step 4: Commit**

```bash
git add backend/PriceTrackerCloud.API/appsettings.json backend/PriceTrackerCloud.Infrastructure/PriceTrackerCloud.Infrastructure.csproj
git commit -m "chore: add PriceChecker config section and Hosting.Abstractions package"
```

---

## Task 2: Skeleton de `PriceCheckerService` + `InternalsVisibleTo`

**Files:**
- Create: `backend/PriceTrackerCloud.Infrastructure/InternalsVisibleTo.cs`
- Create: `backend/PriceTrackerCloud.Infrastructure/BackgroundServices/PriceCheckerService.cs`

- [ ] **Step 1: Crear `InternalsVisibleTo.cs` para exponer internals al proyecto de tests**

Crear `backend/PriceTrackerCloud.Infrastructure/InternalsVisibleTo.cs`:

```csharp
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PriceTrackerCloud.Tests")]
```

Esto permite que el proyecto Tests acceda a miembros `internal` de Infrastructure (necesario para llamar a `RunIterationAsync` directamente desde los tests).

- [ ] **Step 2: Crear `PriceCheckerService.cs` (skeleton que compila)**

Crear `backend/PriceTrackerCloud.Infrastructure/BackgroundServices/PriceCheckerService.cs`:

```csharp
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

            var intervalMinutes = _configuration.GetValue<int>("PriceChecker:IntervalMinutes", 60);
            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
        }
    }

    internal async Task RunIterationAsync(IUnitOfWork uow, CancellationToken ct = default)
    {
        // Implementación en Task 3
        await Task.CompletedTask;
    }

    protected virtual decimal SimulateNewPrice(decimal currentPrice) => currentPrice;
}
```

- [ ] **Step 3: Verificar que compila**

```bash
dotnet build PriceTrackerCloud.sln
```

Resultado esperado:
```
Build succeeded.
0 Error(s)
```

- [ ] **Step 4: Commit**

```bash
git add backend/PriceTrackerCloud.Infrastructure/InternalsVisibleTo.cs backend/PriceTrackerCloud.Infrastructure/BackgroundServices/PriceCheckerService.cs
git commit -m "chore: add PriceCheckerService skeleton and InternalsVisibleTo"
```

---

## Task 3: Tests (en rojo)

**Files:**
- Create: `backend/PriceTrackerCloud.Tests/BackgroundServices/PriceCheckerServiceTests.cs`

- [ ] **Step 1: Crear el archivo de tests**

Crear `backend/PriceTrackerCloud.Tests/BackgroundServices/PriceCheckerServiceTests.cs`:

```csharp
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

    // Subclase de prueba: fija el precio simulado y expone RunIterationAsync (internal)
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

        // Expone RunIterationAsync (internal en Infrastructure, visible vía InternalsVisibleTo)
        public Task RunAsync(IUnitOfWork uow) => RunIterationAsync(uow);
    }
}
```

- [ ] **Step 2: Ejecutar los tests y verificar que fallan**

```bash
dotnet test PriceTrackerCloud.sln --filter "FullyQualifiedName~BackgroundServices"
```

Resultado esperado: los 2 tests **FAIL** porque `RunIterationAsync` aún no tiene implementación real.

- [ ] **Step 3: Commit del test en rojo**

```bash
git add backend/PriceTrackerCloud.Tests/BackgroundServices/PriceCheckerServiceTests.cs
git commit -m "test: add PriceCheckerService tests (red)"
```

---

## Task 4: Implementar `RunIterationAsync` (tests en verde)

**Files:**
- Modify: `backend/PriceTrackerCloud.Infrastructure/BackgroundServices/PriceCheckerService.cs`

- [ ] **Step 1: Reemplazar el body de `RunIterationAsync` con la implementación real**

Sustituir el método `RunIterationAsync` y `SimulateNewPrice` en `PriceCheckerService.cs` por:

```csharp
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
```

El archivo completo resultante debe quedar así:

```csharp
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

            var intervalMinutes = _configuration.GetValue<int>("PriceChecker:IntervalMinutes", 60);
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
```

- [ ] **Step 2: Ejecutar los tests de BackgroundServices**

```bash
dotnet test PriceTrackerCloud.sln --filter "FullyQualifiedName~BackgroundServices"
```

Resultado esperado:
```
Passed  RunIterationAsync_WhenPriceDropsBelowTarget_DeactivatesAlert
Passed  RunIterationAsync_WhenPriceAboveTarget_AlertRemainsActive

Total: 2  Passed: 2  Failed: 0
```

- [ ] **Step 3: Ejecutar todos los tests**

```bash
dotnet test PriceTrackerCloud.sln
```

Resultado esperado:
```
Total: 29  Passed: 29  Failed: 0
```

- [ ] **Step 4: Commit**

```bash
git add backend/PriceTrackerCloud.Infrastructure/BackgroundServices/PriceCheckerService.cs
git commit -m "feat: implement PriceCheckerService with price simulation and alert checking"
```

---

## Task 5: Registrar el servicio en DI y verificar en ejecución

**Files:**
- Modify: `backend/PriceTrackerCloud.Infrastructure/DependencyInjection.cs`

- [ ] **Step 1: Añadir `AddHostedService` en `DependencyInjection.cs`**

Reemplazar el contenido de `backend/PriceTrackerCloud.Infrastructure/DependencyInjection.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Infrastructure.Auth;
using PriceTrackerCloud.Infrastructure.BackgroundServices;
using PriceTrackerCloud.Infrastructure.Data;

namespace PriceTrackerCloud.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PriceTrackerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddHostedService<PriceCheckerService>();

        return services;
    }
}
```

- [ ] **Step 2: Verificar que compila**

```bash
dotnet build PriceTrackerCloud.sln
```

Resultado esperado:
```
Build succeeded.
0 Error(s)
```

- [ ] **Step 3: Smoke test — arrancar la API y verificar los logs**

Requisito previo: PostgreSQL corriendo (ver CLAUDE.md para el comando Docker).

```bash
dotnet run --project backend/PriceTrackerCloud.API
```

En los logs de Serilog deben aparecer al inicio (dentro del primer minuto):
```
[INF] PriceCheckerService: iniciando revisión de precios
[INF] Insertados 6 nuevos registros de precio
[INF] Revisión completada: ninguna alerta disparada
```

(Si no hay alertas activas en BD, aparecerá "ninguna alerta activa". Con alertas cuyo TargetPrice sea alto, aparecerá "Alerta {id} disparada".)

Si PostgreSQL no está disponible, verificar al menos que la API arranca sin error de compilación y que el log `"PriceCheckerService: iniciando revisión de precios"` aparece aunque falle la conexión a BD.

- [ ] **Step 4: Commit final**

```bash
git add backend/PriceTrackerCloud.Infrastructure/DependencyInjection.cs
git commit -m "feat: register PriceCheckerService as hosted service"
```

---

## Checklist de criterios de aceptación

- [ ] `dotnet build` → 0 errores
- [ ] `dotnet test` → 29/29 en verde (27 anteriores + 2 nuevos)
- [ ] `dotnet run` → logs de Serilog muestran el inicio del PriceCheckerService
- [ ] Intervalo configurable en `appsettings.json` (`PriceChecker:IntervalMinutes`)
- [ ] Sin nueva migración de EF Core (no hay cambios de esquema)
