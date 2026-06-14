# Fase 3 — API Layer Design

**Fecha:** 2026-06-14
**Proyecto:** PriceTracker Cloud (TFG)
**Alcance:** Implementación completa de la capa API: Program.cs, DI, autenticación JWT, controllers, middleware de errores, Swagger y Serilog.

---

## Decisiones clave

- **Enfoque:** Controllers clásicos + `DependencyInjection.cs` por capa (patrón existente en Application e Infrastructure).
- **Identificación de usuario:** `UserId` extraído del claim `sub` del JWT (`ClaimTypes.NameIdentifier`).
- **Formato de errores:** Problem Details (RFC 7807) vía middleware global.
- **Logging:** Serilog → consola únicamente (compatible con Azure App Service / contenedores).

---

## 1. Estructura de archivos

### Archivos nuevos en `PriceTrackerCloud.API/`

```
PriceTrackerCloud.API/
├── Program.cs                          ← reemplazar completamente
├── DependencyInjection.cs              ← NUEVO
├── Middleware/
│   └── ErrorHandlingMiddleware.cs      ← NUEVO
└── Controllers/
    ├── AuthController.cs               ← NUEVO
    ├── ProductsController.cs           ← NUEVO
    ├── PricesController.cs             ← NUEVO
    └── AlertsController.cs             ← NUEVO
```

### Archivos nuevos/modificados en `PriceTrackerCloud.Infrastructure/`

```
Infrastructure/
├── DependencyInjection.cs              ← modificar: añadir BCryptPasswordHasher + JwtTokenGenerator
└── Auth/
    ├── BCryptPasswordHasher.cs         ← NUEVO
    └── JwtTokenGenerator.cs           ← NUEVO
```

---

## 2. Infrastructure: implementaciones de auth

### `BCryptPasswordHasher`
- Implementa `IPasswordHasher` (definida en Application).
- Paquete: `BCrypt.Net-Next`.
- `Hash(password)` → `BCrypt.HashPassword(password)`.
- `Verify(hash, password)` → `BCrypt.Verify(password, hash)`.
- Registrada como `Scoped` en `Infrastructure/DependencyInjection.cs`.

### `JwtTokenGenerator`
- Implementa `IJwtTokenGenerator` (definida en Application).
- Paquete: `System.IdentityModel.Tokens.Jwt` (ya en el SDK de ASP.NET Core).
- Lee `JwtSettings` (SecretKey, Issuer, Audience, ExpirationMinutes) de `IConfiguration`.
- Claims del token: `sub` (UserId.ToString()), `email`, `name`, `role`.
- Registrada como `Scoped`.

### Cambio en `Infrastructure/DependencyInjection.cs`
Añadir al método `AddInfrastructure`:
```csharp
services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
```

---

## 3. API: DependencyInjection.cs

Método de extensión `AddApiServices(IConfiguration configuration)` que registra:

1. `AddControllers()`.
2. JWT Bearer Authentication con `JwtSettings` del `appsettings.json`:
   - `ValidateIssuer = true`, `ValidateAudience = true`, `ValidateLifetime = true`, `ValidateIssuerSigningKey = true`.
3. Swagger/OpenAPI con definición de seguridad `Bearer` para poder enviar el token desde la UI de Swagger.
4. CORS: política `AllowAll` habilitada en desarrollo (para Vite en Fase 5).
5. Serilog configurado para consola con `OutputTemplate` enriquecido (timestamp, level, message, exception).

---

## 4. Program.cs

```csharp
// Serilog se configura antes que el builder
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

// Pipeline
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("AllowAll");         // solo en desarrollo
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
```

---

## 5. Controllers

Todos heredan de `ControllerBase` con `[ApiController]`. La política de autorización por defecto es `[Authorize]`; los endpoints públicos llevan `[AllowAnonymous]`.

### `AuthController` — `/auth`

| Método | Ruta | Handler | Auth |
|--------|------|---------|------|
| POST | `/auth/register` | `RegisterUserCommand` | `[AllowAnonymous]` |
| POST | `/auth/login` | `LoginUserCommand` | `[AllowAnonymous]` |

Retorna `201 Created` en register, `200 OK` con `AuthResponseDto` en login.

### `ProductsController` — `/products`

| Método | Ruta | Handler | Auth |
|--------|------|---------|------|
| GET | `/products` | `GetProductsQuery` | `[Authorize]` |
| GET | `/products/{id}` | `GetProductByIdQuery` | `[Authorize]` |
| POST | `/products` | `CreateProductCommand` | `[Authorize]` |

Retorna `200 OK` en GETs, `201 Created` en POST.

### `PricesController` — `/prices`

| Método | Ruta | Handler | Auth |
|--------|------|---------|------|
| GET | `/prices/history/{productId}` | `GetPriceHistoryQuery` | `[Authorize]` |

Retorna `200 OK`.

### `AlertsController` — `/alerts`

| Método | Ruta | Handler | Auth |
|--------|------|---------|------|
| GET | `/alerts` | `GetUserAlertsQuery` | `[Authorize]` |
| POST | `/alerts` | `CreateAlertCommand` | `[Authorize]` |
| DELETE | `/alerts/{id}` | `DeleteAlertCommand` | `[Authorize]` |

El `UserId` se extrae del JWT: `Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)`.
Retorna `200 OK` en GET, `201 Created` en POST, `204 NoContent` en DELETE.

---

## 6. ErrorHandlingMiddleware

Captura excepciones no controladas en el pipeline y las convierte en Problem Details (RFC 7807). Todas se loguean con Serilog (`_logger.LogError`) antes de responder.

| Excepción | Status HTTP | Title |
|-----------|-------------|-------|
| `ValidationException` (FluentValidation) | 400 | Validation Error |
| `UnauthorizedAccessException` | 401 | Unauthorized |
| `KeyNotFoundException` | 404 | Not Found |
| `Exception` (cualquier otra) | 500 | Internal Server Error |

El body de error sigue el formato Problem Details:
```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Validation Error",
  "status": 400,
  "detail": "El nombre del producto es obligatorio."
}
```
Para `ValidationException`, el campo `detail` concatena todos los errores de FluentValidation.

---

## 7. Paquetes NuGet a añadir

| Proyecto | Paquete |
|----------|---------|
| `Infrastructure` | `BCrypt.Net-Next` |
| `API` | `Swashbuckle.AspNetCore` |
| `API` | `Serilog.AspNetCore` |
| `API` | `Serilog.Sinks.Console` |
| `API` | `Microsoft.AspNetCore.Authentication.JwtBearer` |

> `System.IdentityModel.Tokens.Jwt` viene incluido transitivamente con `JwtBearer`, no hace falta añadirlo explícitamente.

---

## 8. Criterios de aceptación

- `dotnet run` desde `PriceTrackerCloud.API` arranca sin errores.
- `GET /swagger` muestra la UI con todos los endpoints documentados y el botón "Authorize" para el JWT.
- `POST /auth/register` y `POST /auth/login` devuelven token JWT válido.
- Endpoints protegidos devuelven `401` sin token y `200/201/204` con token válido.
- Un request con datos inválidos devuelve `400` con Problem Details.
- Los logs aparecen en consola con formato Serilog (timestamp + level + message).
- `dotnet test` sigue en verde (20/20).
