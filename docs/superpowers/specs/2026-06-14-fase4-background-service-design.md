# Fase 4 — BackgroundService de revisión de precios y alertas

**Fecha:** 2026-06-14  
**Estado:** Aprobado  
**Proyecto:** PriceTracker Cloud (TFG)

---

## Objetivo

Implementar un `BackgroundService` que se ejecute periódicamente para:
1. Simular precios nuevos (±10% sobre el último precio conocido) e insertarlos en BD
2. Comparar los precios resultantes contra las alertas activas de los usuarios
3. Desactivar (`IsActive = false`) las alertas cuyo precio objetivo ha sido alcanzado
4. Registrar cada evento relevante con Serilog

---

## Decisiones de diseño

| Decisión | Elección | Razón |
|----------|----------|-------|
| Enfoque | Un único `BackgroundService` | Simple, directo, justificable en TFG |
| Precio actual | Generado aleatoriamente ±10% sobre último precio + insertado en BD | Simula que el scraper ya funcionó |
| Intervalo | Configurable en `appsettings.json` (`PriceChecker:IntervalMinutes`) | Flexibilidad sin complejidad |
| Alertas disparadas | `IsActive = false` + log | Opción c: desactivar, dejando endpoint futuro para reactivar |
| Scope de EF Core | `IServiceScopeFactory` por iteración | Patrón estándar para BackgroundService con DbContext scoped |

---

## Componentes

### Nuevo archivo

```
backend/PriceTrackerCloud.Infrastructure/BackgroundServices/PriceCheckerService.cs
```

`PriceCheckerService : BackgroundService` — toda la lógica del job reside aquí.

### Archivos modificados

| Archivo | Cambio |
|---------|--------|
| `Infrastructure/DependencyInjection.cs` | Añadir `services.AddHostedService<PriceCheckerService>()` |
| `API/appsettings.json` | Añadir sección `"PriceChecker": { "IntervalMinutes": 60 }` |

### Nuevo test

```
backend/PriceTrackerCloud.Tests/BackgroundServices/PriceCheckerServiceTests.cs
```

---

## Configuración

```json
"PriceChecker": {
  "IntervalMinutes": 60
}
```

El servicio lee este valor en cada iteración via `IConfiguration`. Valor por defecto: 60 minutos.

---

## Flujo de ejecución (por iteración)

```
PriceCheckerService.ExecuteAsync  ← loop hasta stoppingToken
│
├── try
│   ├── 1. Crear IServiceScope → obtener IUnitOfWork
│   ├── 2. Obtener todos los ProductPrice existentes (agrupados por ProductId + StoreId)
│   ├── 3. Por cada par (ProductId, StoreId):
│   │       precio = últimoPrecio × random(0.90, 1.10)
│   │       insertar nuevo ProductPrice con DateCollected = UtcNow
│   ├── 4. SaveChangesAsync  ← transacción única para todos los precios nuevos
│   ├── 5. GetActiveAlertsAsync
│   ├── 6. Por cada alerta activa:
│   │       buscar en los precios recién insertados (en memoria) si alguno con
│   │       ProductId == alerta.ProductId tiene Price ≤ alerta.TargetPrice
│   │       si sí:
│   │           alerta.IsActive = false
│   │           precioDisparador = min(precios nuevos del producto)
│   │           Log.Information("Alerta {AlertId} disparada — producto {ProductId}, precio {Price} ≤ objetivo {Target}")
│   ├── 7. SaveChangesAsync  ← transacción única para todas las alertas disparadas
│   └── 8. Dispose del scope
│
├── catch (Exception ex)
│   └── Log.Error(ex, "Error en PriceCheckerService durante la iteración")
│       (el loop continúa — el servicio no tumba la API)
│
└── await Task.Delay(IntervalMinutes * 60_000, stoppingToken)
```

---

## Logging (Serilog)

| Evento | Nivel | Mensaje |
|--------|-------|---------|
| Inicio de iteración | Information | `"PriceCheckerService: iniciando revisión de precios"` |
| Precios insertados | Information | `"Insertados {Count} nuevos registros de precio"` |
| Alerta disparada | Information | `"Alerta {AlertId} disparada — ProductId {ProductId}, precio {Price} ≤ objetivo {Target}"` |
| Sin alertas disparadas | Information | `"Revisión completada: ninguna alerta disparada"` |
| Error en iteración | Error | `"Error en PriceCheckerService durante la iteración"` + excepción |

---

## Tests

**Archivo:** `Tests/BackgroundServices/PriceCheckerServiceTests.cs`

| Test | Escenario | Assert |
|------|-----------|--------|
| `ExecuteAsync_WhenPriceDropsBelowTarget_DeactivatesAlert` | precio simulado ≤ TargetPrice | `alert.IsActive == false` + `SaveChangesAsync` llamado |
| `ExecuteAsync_WhenPriceAboveTarget_AlertRemainsActive` | precio simulado > TargetPrice | `alert.IsActive == true` + `SaveChangesAsync` no llamado para alertas |

Los tests mockean `IServiceScopeFactory` → `IServiceProvider` → `IUnitOfWork` → repositorios con Moq.  
La aleatoriedad del precio se controla mediante una semilla fija (`Random(seed)`) inyectada en el constructor para poder predecir el precio generado en tests.

---

## Interfaz futura (Fase 5+)

El campo `IsActive` ya existe en `Alert`. Para que el usuario pueda reactivar alertas desde el frontend bastará con añadir un endpoint `PATCH /alerts/{id}/reactivate` en una fase posterior — no requiere cambios de esquema.

---

## Criterios de aceptación

- `dotnet run` arranca y el servicio aparece en los logs de Serilog al inicio
- Cada `IntervalMinutes` minutos se insertan nuevos `ProductPrice` en BD
- Cuando un precio ≤ `TargetPrice`: la alerta pasa a `IsActive = false` y aparece en los logs
- `dotnet test` → todos los tests en verde (27 existentes + 2 nuevos = 29 total)
- Sin cambios de esquema (no se necesita nueva migración)
