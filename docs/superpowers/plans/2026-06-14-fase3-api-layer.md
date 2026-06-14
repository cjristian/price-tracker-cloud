# Fase 3 — API Layer Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implementar la capa API completa: BCrypt + JWT en Infrastructure, controllers, middleware de errores (Problem Details), Swagger y Serilog consola.

**Architecture:** Controllers clásicos delgados que despachan a MediatR. Cada capa expone su propio `DependencyInjection.cs`. El middleware global intercepta excepciones y las convierte en Problem Details (RFC 7807). Serilog escribe a consola (compatible con Azure App Service).

**Tech Stack:** ASP.NET Core 9, MediatR 12, FluentValidation 11, BCrypt.Net-Next, Swashbuckle.AspNetCore, Serilog.AspNetCore, Microsoft.AspNetCore.Authentication.JwtBearer.

---

## Mapa de archivos

| Acción | Ruta |
|--------|------|
| Modificar | `backend/PriceTrackerCloud.Infrastructure/DependencyInjection.cs` |
| Crear | `backend/PriceTrackerCloud.Infrastructure/Auth/BCryptPasswordHasher.cs` |
| Crear | `backend/PriceTrackerCloud.Infrastructure/Auth/JwtTokenGenerator.cs` |
| Reemplazar | `backend/PriceTrackerCloud.API/Program.cs` |
| Crear | `backend/PriceTrackerCloud.API/DependencyInjection.cs` |
| Crear | `backend/PriceTrackerCloud.API/Middleware/ErrorHandlingMiddleware.cs` |
| Crear | `backend/PriceTrackerCloud.API/Controllers/AuthController.cs` |
| Crear | `backend/PriceTrackerCloud.API/Controllers/ProductsController.cs` |
| Crear | `backend/PriceTrackerCloud.API/Controllers/PricesController.cs` |
| Crear | `backend/PriceTrackerCloud.API/Controllers/AlertsController.cs` |
| Crear | `backend/PriceTrackerCloud.Tests/Infrastructure/BCryptPasswordHasherTests.cs` |
| Crear | `backend/PriceTrackerCloud.Tests/Infrastructure/JwtTokenGeneratorTests.cs` |

---

## Task 1: Añadir paquetes NuGet

**Files:**
- Modify: `backend/PriceTrackerCloud.Infrastructure/PriceTrackerCloud.Infrastructure.csproj`
- Modify: `backend/PriceTrackerCloud.API/PriceTrackerCloud.API.csproj`

- [ ] **Step 1: Añadir BCrypt al proyecto Infrastructure**

Ejecutar desde `backend/`:
```bash
dotnet add PriceTrackerCloud.Infrastructure/PriceTrackerCloud.Infrastructure.csproj package BCrypt.Net-Next
```
Expected: `PackageReference` para `BCrypt.Net-Next` aparece en el `.csproj`.

- [ ] **Step 2: Añadir paquetes al proyecto API**

```bash
dotnet add PriceTrackerCloud.API/PriceTrackerCloud.API.csproj package Swashbuckle.AspNetCore
dotnet add PriceTrackerCloud.API/PriceTrackerCloud.API.csproj package Serilog.AspNetCore
dotnet add PriceTrackerCloud.API/PriceTrackerCloud.API.csproj package Serilog.Sinks.Console
dotnet add PriceTrackerCloud.API/PriceTrackerCloud.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
```
Expected: los cuatro `PackageReference` aparecen en el `.csproj`.

- [ ] **Step 3: Verificar que la solución compila**

```bash
dotnet build PriceTrackerCloud.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 4: Commit**

```bash
git add backend/PriceTrackerCloud.Infrastructure/PriceTrackerCloud.Infrastructure.csproj
git add backend/PriceTrackerCloud.API/PriceTrackerCloud.API.csproj
git commit -m "chore: add BCrypt, Swashbuckle, Serilog and JwtBearer packages"
```

---

## Task 2: BCryptPasswordHasher

**Files:**
- Create: `backend/PriceTrackerCloud.Infrastructure/Auth/BCryptPasswordHasher.cs`
- Create: `backend/PriceTrackerCloud.Tests/Infrastructure/BCryptPasswordHasherTests.cs`

- [ ] **Step 1: Escribir el test que falla**

Crear `backend/PriceTrackerCloud.Tests/Infrastructure/BCryptPasswordHasherTests.cs`:

```csharp
using FluentAssertions;
using PriceTrackerCloud.Infrastructure.Auth;

namespace PriceTrackerCloud.Tests.Infrastructure;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _sut = new();

    [Fact]
    public void Hash_ShouldReturnNonEmptyString()
    {
        var hash = _sut.Hash("mypassword");
        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Hash_SamePassword_ShouldProduceDifferentHashes()
    {
        var hash1 = _sut.Hash("mypassword");
        var hash2 = _sut.Hash("mypassword");
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Verify_CorrectPassword_ShouldReturnTrue()
    {
        var hash = _sut.Hash("mypassword");
        _sut.Verify(hash, "mypassword").Should().BeTrue();
    }

    [Fact]
    public void Verify_WrongPassword_ShouldReturnFalse()
    {
        var hash = _sut.Hash("mypassword");
        _sut.Verify(hash, "wrongpassword").Should().BeFalse();
    }
}
```

- [ ] **Step 2: Ejecutar el test para confirmar que falla**

```bash
dotnet test PriceTrackerCloud.Tests/PriceTrackerCloud.Tests.csproj --filter "BCryptPasswordHasherTests"
```
Expected: error de compilación — `BCryptPasswordHasher` no existe aún.

- [ ] **Step 3: Implementar BCryptPasswordHasher**

Crear `backend/PriceTrackerCloud.Infrastructure/Auth/BCryptPasswordHasher.cs`:

```csharp
using PriceTrackerCloud.Application.Interfaces;

namespace PriceTrackerCloud.Infrastructure.Auth;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string hash, string password) => BCrypt.Net.BCrypt.Verify(password, hash);
}
```

- [ ] **Step 4: Ejecutar los tests y confirmar que pasan**

```bash
dotnet test PriceTrackerCloud.Tests/PriceTrackerCloud.Tests.csproj --filter "BCryptPasswordHasherTests"
```
Expected: `Total: 4  Passed: 4  Failed: 0`

- [ ] **Step 5: Commit**

```bash
git add backend/PriceTrackerCloud.Infrastructure/Auth/BCryptPasswordHasher.cs
git add backend/PriceTrackerCloud.Tests/Infrastructure/BCryptPasswordHasherTests.cs
git commit -m "feat: implement BCryptPasswordHasher"
```

---

## Task 3: JwtTokenGenerator

**Files:**
- Create: `backend/PriceTrackerCloud.Infrastructure/Auth/JwtTokenGenerator.cs`
- Create: `backend/PriceTrackerCloud.Tests/Infrastructure/JwtTokenGeneratorTests.cs`

- [ ] **Step 1: Escribir el test que falla**

Crear `backend/PriceTrackerCloud.Tests/Infrastructure/JwtTokenGeneratorTests.cs`:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using PriceTrackerCloud.Domain.Entities;
using PriceTrackerCloud.Domain.Enums;
using PriceTrackerCloud.Infrastructure.Auth;

namespace PriceTrackerCloud.Tests.Infrastructure;

public class JwtTokenGeneratorTests
{
    private readonly JwtTokenGenerator _sut;

    public JwtTokenGeneratorTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "super-secret-key-for-testing-minimum-256-bits!!",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpirationMinutes"] = "60"
            })
            .Build();

        _sut = new JwtTokenGenerator(config);
    }

    [Fact]
    public void GenerateToken_ShouldReturnNonEmptyString()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Ana", Email = "ana@test.com", Role = UserRole.User };
        var token = _sut.GenerateToken(user);
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_ShouldContainUserIdClaim()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "Ana", Email = "ana@test.com", Role = UserRole.User };

        var token = _sut.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value
            .Should().Be(userId.ToString());
    }

    [Fact]
    public void GenerateToken_ShouldContainEmailClaim()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Ana", Email = "ana@test.com", Role = UserRole.User };

        var token = _sut.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value
            .Should().Be("ana@test.com");
    }
}
```

- [ ] **Step 2: Ejecutar el test para confirmar que falla**

```bash
dotnet test PriceTrackerCloud.Tests/PriceTrackerCloud.Tests.csproj --filter "JwtTokenGeneratorTests"
```
Expected: error de compilación — `JwtTokenGenerator` no existe aún.

- [ ] **Step 3: Implementar JwtTokenGenerator**

Crear `backend/PriceTrackerCloud.Infrastructure/Auth/JwtTokenGenerator.cs`:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Infrastructure.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"]!;
        var issuer    = _configuration["JwtSettings:Issuer"]!;
        var audience  = _configuration["JwtSettings:Audience"]!;
        var minutes   = int.Parse(_configuration["JwtSettings:ExpirationMinutes"]!);

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name,  user.Name),
            new Claim(ClaimTypes.Role,               user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

- [ ] **Step 4: Ejecutar los tests y confirmar que pasan**

```bash
dotnet test PriceTrackerCloud.Tests/PriceTrackerCloud.Tests.csproj --filter "JwtTokenGeneratorTests"
```
Expected: `Total: 3  Passed: 3  Failed: 0`

- [ ] **Step 5: Commit**

```bash
git add backend/PriceTrackerCloud.Infrastructure/Auth/JwtTokenGenerator.cs
git add backend/PriceTrackerCloud.Tests/Infrastructure/JwtTokenGeneratorTests.cs
git commit -m "feat: implement JwtTokenGenerator"
```

---

## Task 4: Registrar BCryptPasswordHasher y JwtTokenGenerator en Infrastructure DI

**Files:**
- Modify: `backend/PriceTrackerCloud.Infrastructure/DependencyInjection.cs`

- [ ] **Step 1: Añadir los registros**

Editar `backend/PriceTrackerCloud.Infrastructure/DependencyInjection.cs`. El archivo debe quedar así:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Infrastructure.Auth;
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

        return services;
    }
}
```

- [ ] **Step 2: Verificar que todos los tests siguen en verde**

```bash
dotnet test PriceTrackerCloud.sln
```
Expected: `Total: 27  Passed: 27  Failed: 0` (20 existentes + 4 BCrypt + 3 JWT)

- [ ] **Step 3: Commit**

```bash
git add backend/PriceTrackerCloud.Infrastructure/DependencyInjection.cs
git commit -m "feat: register BCryptPasswordHasher and JwtTokenGenerator in Infrastructure DI"
```

---

## Task 5: ErrorHandlingMiddleware

**Files:**
- Create: `backend/PriceTrackerCloud.API/Middleware/ErrorHandlingMiddleware.cs`

- [ ] **Step 1: Crear el middleware**

Crear `backend/PriceTrackerCloud.API/Middleware/ErrorHandlingMiddleware.cs`:

```csharp
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PriceTrackerCloud.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, ex);
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
    {
        var (status, title, detail) = ex switch
        {
            ValidationException ve        => (400, "Validation Error",
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            UnauthorizedAccessException   => (401, "Unauthorized",      ex.Message),
            InvalidOperationException     => (409, "Conflict",          ex.Message),
            KeyNotFoundException          => (404, "Not Found",         ex.Message),
            _                            => (500, "Internal Server Error", "An unexpected error occurred.")
        };

        var problem = new ProblemDetails
        {
            Type   = "https://tools.ietf.org/html/rfc7807",
            Title  = title,
            Status = status,
            Detail = detail
        };

        context.Response.StatusCode  = status;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }
}
```

- [ ] **Step 2: Compilar para verificar que no hay errores**

```bash
dotnet build PriceTrackerCloud.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add backend/PriceTrackerCloud.API/Middleware/ErrorHandlingMiddleware.cs
git commit -m "feat: add ErrorHandlingMiddleware with Problem Details (RFC 7807)"
```

---

## Task 6: API DependencyInjection.cs

**Files:**
- Create: `backend/PriceTrackerCloud.API/DependencyInjection.cs`

- [ ] **Step 1: Crear el archivo**

Crear `backend/PriceTrackerCloud.API/DependencyInjection.cs`:

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace PriceTrackerCloud.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        // JWT Authentication
        var secretKey = configuration["JwtSettings:SecretKey"]!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = configuration["JwtSettings:Issuer"],
                    ValidAudience            = configuration["JwtSettings:Audience"],
                    IssuerSigningKey         = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };
            });

        services.AddAuthorization();

        // Swagger con soporte para JWT Bearer
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title   = "PriceTracker Cloud API",
                Version = "v1"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "Bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Introduce el token JWT. Ejemplo: Bearer {token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // CORS abierto para desarrollo (Vite en Fase 5)
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

        return services;
    }
}
```

- [ ] **Step 2: Compilar**

```bash
dotnet build PriceTrackerCloud.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add backend/PriceTrackerCloud.API/DependencyInjection.cs
git commit -m "feat: add API DependencyInjection with JWT, Swagger and CORS"
```

---

## Task 7: Program.cs

**Files:**
- Modify: `backend/PriceTrackerCloud.API/Program.cs`

- [ ] **Step 1: Reemplazar Program.cs**

Reemplazar el contenido completo de `backend/PriceTrackerCloud.API/Program.cs`:

```csharp
using PriceTrackerCloud.API;
using PriceTrackerCloud.API.Middleware;
using PriceTrackerCloud.Application;
using PriceTrackerCloud.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

- [ ] **Step 2: Compilar**

```bash
dotnet build PriceTrackerCloud.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add backend/PriceTrackerCloud.API/Program.cs
git commit -m "feat: wire up Program.cs with Serilog, middleware and full DI"
```

---

## Task 8: AuthController

**Files:**
- Create: `backend/PriceTrackerCloud.API/Controllers/AuthController.cs`

- [ ] **Step 1: Crear el controller**

Crear `backend/PriceTrackerCloud.API/Controllers/AuthController.cs`:

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerCloud.Application.Commands.Auth;
using PriceTrackerCloud.Application.DTOs.Auth;

namespace PriceTrackerCloud.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _sender.Send(
            new RegisterUserCommand(dto.Name, dto.Email, dto.Password));
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _sender.Send(
            new LoginUserCommand(dto.Email, dto.Password));
        return Ok(result);
    }
}
```

- [ ] **Step 2: Compilar**

```bash
dotnet build PriceTrackerCloud.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add backend/PriceTrackerCloud.API/Controllers/AuthController.cs
git commit -m "feat: add AuthController (register + login)"
```

---

## Task 9: ProductsController

**Files:**
- Create: `backend/PriceTrackerCloud.API/Controllers/ProductsController.cs`

- [ ] **Step 1: Crear el controller**

Crear `backend/PriceTrackerCloud.API/Controllers/ProductsController.cs`:

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerCloud.Application.Commands.Products;
using PriceTrackerCloud.Application.DTOs.Products;
using PriceTrackerCloud.Application.Queries.Products;

namespace PriceTrackerCloud.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new GetProductsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetProductByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await _sender.Send(
            new CreateProductCommand(dto.Name, dto.Description, dto.Category));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
```

- [ ] **Step 2: Compilar**

```bash
dotnet build PriceTrackerCloud.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add backend/PriceTrackerCloud.API/Controllers/ProductsController.cs
git commit -m "feat: add ProductsController (GET list, GET by id, POST)"
```

---

## Task 10: PricesController

**Files:**
- Create: `backend/PriceTrackerCloud.API/Controllers/PricesController.cs`

- [ ] **Step 1: Crear el controller**

Crear `backend/PriceTrackerCloud.API/Controllers/PricesController.cs`:

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerCloud.Application.Queries.Prices;

namespace PriceTrackerCloud.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PricesController : ControllerBase
{
    private readonly ISender _sender;

    public PricesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("history/{productId:guid}")]
    public async Task<IActionResult> GetHistory(Guid productId)
    {
        var result = await _sender.Send(new GetPriceHistoryQuery(productId));
        return Ok(result);
    }
}
```

- [ ] **Step 2: Compilar**

```bash
dotnet build PriceTrackerCloud.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add backend/PriceTrackerCloud.API/Controllers/PricesController.cs
git commit -m "feat: add PricesController (GET history by productId)"
```

---

## Task 11: AlertsController

**Files:**
- Create: `backend/PriceTrackerCloud.API/Controllers/AlertsController.cs`

- [ ] **Step 1: Crear el controller**

Crear `backend/PriceTrackerCloud.API/Controllers/AlertsController.cs`:

```csharp
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerCloud.Application.Commands.Alerts;
using PriceTrackerCloud.Application.DTOs.Alerts;
using PriceTrackerCloud.Application.Queries.Alerts;

namespace PriceTrackerCloud.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly ISender _sender;

    public AlertsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _sender.Send(new GetUserAlertsQuery(userId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlertDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _sender.Send(
            new CreateAlertCommand(userId, dto.ProductId, dto.TargetPrice));
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _sender.Send(new DeleteAlertCommand(id, userId));
        return NoContent();
    }
}
```

- [ ] **Step 2: Compilar**

```bash
dotnet build PriceTrackerCloud.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Commit**

```bash
git add backend/PriceTrackerCloud.API/Controllers/AlertsController.cs
git commit -m "feat: add AlertsController (GET, POST, DELETE) with UserId from JWT claim"
```

---

## Task 12: Verificación final

- [ ] **Step 1: Ejecutar todos los tests**

```bash
dotnet test PriceTrackerCloud.sln
```
Expected: `Total: 27  Passed: 27  Failed: 0`

- [ ] **Step 2: Levantar el servidor**

```bash
dotnet run --project PriceTrackerCloud.API/PriceTrackerCloud.API.csproj
```
Expected: logs de Serilog en consola con el formato `[HH:mm:ss INF] Now listening on: http://localhost:5XXX`

> Nota: la API intentará conectar con PostgreSQL al arrancar. Si la BD no está disponible localmente, `dotnet run` arrancará igual pero los endpoints que tocan la BD devolverán 500. Para probar contra la BD, levanta PostgreSQL con Docker:
> ```bash
> docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=pricetracker postgres:16
> dotnet ef database update --project PriceTrackerCloud.Infrastructure --startup-project PriceTrackerCloud.API
> ```

- [ ] **Step 3: Verificar Swagger**

Abrir en el navegador: `http://localhost:{puerto}/swagger`

Verificar:
- Aparecen los 4 controllers con todos sus endpoints.
- El botón "Authorize" permite introducir el token Bearer.
- Los endpoints sin `[AllowAnonymous]` muestran el icono de candado.

- [ ] **Step 4: Smoke test manual de auth**

En Swagger, ejecutar `POST /auth/register`:
```json
{ "name": "Test User", "email": "test@example.com", "password": "Password123!" }
```
Expected: `201 Created` con `AuthResponseDto` que contiene un `token`.

Ejecutar `POST /auth/login` con las mismas credenciales.
Expected: `200 OK` con token JWT.

Copiar el token, pulsar "Authorize" en Swagger, pegar `Bearer {token}`.

Ejecutar `GET /products`.
Expected: `200 OK` con la lista de productos del seed.

- [ ] **Step 5: Commit final**

```bash
git commit --allow-empty -m "chore: fase 3 complete — API layer fully wired"
```
