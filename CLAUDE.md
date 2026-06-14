# PriceTracker Cloud — Contexto para Claude Code

## Qué es este proyecto
Plataforma web para rastrear y comparar precios de productos entre tiendas online.
Incluye histórico de precios, alertas de bajada y dashboards analíticos.
**Tipo:** Trabajo de Fin de Grado (TFG) — proyecto académico, énfasis en buenas prácticas.
**Directorio raíz:** `c:\Users\crich\Desktop\proyecto`

---

## Stack (versiones exactas)
| Capa | Tecnología |
|------|-----------|
| Backend | .NET 9 · ASP.NET Core Web API · EF Core 9 · PostgreSQL 16 |
| Auth | JWT (Bearer token) · BCrypt.Net-Next 4.2.0 · JwtBearer 9.0.5 |
| CQRS | MediatR 12.4.1 |
| Mappings | AutoMapper 13.0.1 |
| Validación | FluentValidation 11.11.0 |
| Logging | Serilog.AspNetCore 10.0.0 + Serilog.Sinks.Console 6.1.1 |
| Jobs | BackgroundService (pendiente Fase 4) |
| Docs | Swashbuckle.AspNetCore 6.9.0 |
| Frontend | React 18 · Vite · TypeScript · Redux Toolkit · TailwindCSS · Recharts |
| HTTP cliente | Axios (con interceptor JWT) |
| Routing | React Router v6 |
| DevOps | Docker · Docker Compose · GitHub Actions · Kubernetes · Azure |
| Tests | xUnit + Moq 4.20.72 + FluentAssertions 6.12.1 |

---

## Arquitectura — Clean Architecture

```
proyecto/
├── CLAUDE.md
├── .gitignore
├── docs/
│   └── superpowers/
│       ├── specs/2026-06-14-fase3-api-design.md
│       └── plans/2026-06-14-fase3-api-layer.md
├── backend/
│   ├── PriceTrackerCloud.sln
│   ├── PriceTrackerCloud.Domain/
│   │   ├── Common/BaseEntity.cs
│   │   ├── Enums/UserRole.cs
│   │   └── Entities/ (User, Product, Store, ProductPrice, Alert)
│   ├── PriceTrackerCloud.Application/
│   │   ├── Behaviors/ValidationBehavior.cs
│   │   ├── Commands/ (Auth, Products, Alerts)
│   │   ├── DTOs/ (Auth, Products, Prices, Alerts)
│   │   ├── Interfaces/ (IUnitOfWork, IPasswordHasher, IJwtTokenGenerator, Repositories/)
│   │   ├── Mappings/MappingProfile.cs
│   │   ├── Queries/ (Products, Prices, Alerts)
│   │   ├── Validators/ (Auth, Products, Alerts)
│   │   └── DependencyInjection.cs
│   ├── PriceTrackerCloud.Infrastructure/
│   │   ├── Auth/
│   │   │   ├── BCryptPasswordHasher.cs   ← implementa IPasswordHasher
│   │   │   └── JwtTokenGenerator.cs      ← implementa IJwtTokenGenerator
│   │   ├── Data/
│   │   │   ├── PriceTrackerDbContext.cs
│   │   │   ├── DesignTimeDbContextFactory.cs
│   │   │   ├── Configurations/ (5 entidades + SeedData.cs)
│   │   │   └── Migrations/ (InitialCreate — generada)
│   │   ├── Repositories/ (Repository<T>, User, Product, Price, Alert)
│   │   ├── UnitOfWork.cs
│   │   └── DependencyInjection.cs
│   ├── PriceTrackerCloud.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs         ← POST /auth/register, POST /auth/login
│   │   │   ├── ProductsController.cs     ← GET/POST /products, GET /products/{id}
│   │   │   ├── PricesController.cs       ← GET /prices/history/{productId}
│   │   │   └── AlertsController.cs       ← GET/POST /alerts, DELETE /alerts/{id}
│   │   ├── Middleware/
│   │   │   └── ErrorHandlingMiddleware.cs ← Problem Details RFC 7807
│   │   ├── DependencyInjection.cs        ← JWT + Swagger + CORS
│   │   ├── Program.cs                    ← Serilog + pipeline completo
│   │   └── appsettings.json              ← connection string + JWT settings
│   └── PriceTrackerCloud.Tests/
│       ├── Validators/ (Register, CreateProduct, CreateAlert — 13 tests)
│       ├── Handlers/ (RegisterUser, GetProducts — 7 tests)
│       └── Infrastructure/ (BCryptPasswordHasher — 4 tests, JwtTokenGenerator — 3 tests)
└── frontend/
    └── src/ (estructura de carpetas lista, código pendiente Fase 5)
```

### Regla de dependencias
```
API ──► Application ──► Domain
Infrastructure ──► Application
Infrastructure ──► Domain
```
> El Domain NO depende de nada externo. Application NO depende de Infrastructure.
> La API referencia Infrastructure porque es el Composition Root (donde se cablea el DI).

---

## Entidades (Domain)

Todas heredan de `BaseEntity` con `Id` de tipo `Guid`.

| Entidad | Campos |
|---------|--------|
| `User` | Name, Email, PasswordHash, Role (enum), CreatedAt |
| `Product` | Name, Description, Category |
| `Store` | Name, Website |
| `ProductPrice` | ProductId (FK), StoreId (FK), Price (decimal), DateCollected |
| `Alert` | UserId (FK), ProductId (FK), TargetPrice (decimal), IsActive, CreatedAt |

---

## Endpoints implementados
| Método | Ruta | Controller | Handler MediatR | Auth |
|--------|------|------------|-----------------|------|
| POST | /auth/register | AuthController | `RegisterUserCommand` | Anónimo |
| POST | /auth/login | AuthController | `LoginUserCommand` | Anónimo |
| GET | /products | ProductsController | `GetProductsQuery` | JWT |
| GET | /products/{id} | ProductsController | `GetProductByIdQuery` | JWT |
| POST | /products | ProductsController | `CreateProductCommand` | JWT |
| GET | /prices/history/{productId} | PricesController | `GetPriceHistoryQuery` | JWT |
| GET | /alerts | AlertsController | `GetUserAlertsQuery` | JWT |
| POST | /alerts | AlertsController | `CreateAlertCommand` | JWT |
| DELETE | /alerts/{id} | AlertsController | `DeleteAlertCommand` | JWT |

El `UserId` en AlertsController se extrae del claim `ClaimTypes.NameIdentifier` del JWT (claim `sub`).

---

## Datos semilla (ya en migración InitialCreate)

**Tiendas (GUIDs fijos):**
- Amazon: `a1a1a1a1-0000-0000-0000-000000000001`
- PcComponentes: `a1a1a1a1-0000-0000-0000-000000000002`
- MediaMarkt: `a1a1a1a1-0000-0000-0000-000000000003`
- El Corte Inglés: `a1a1a1a1-0000-0000-0000-000000000004`

**Productos (GUIDs fijos):**
- PS5: `b2b2b2b2-0000-0000-0000-000000000001`
- iPhone 15 Pro: `b2b2b2b2-0000-0000-0000-000000000002`
- MacBook Pro 14 M3: `b2b2b2b2-0000-0000-0000-000000000003`

36 registros de precios con tendencia bajista (6 snapshots × 2 tiendas × 3 productos).
La migración se aplica con: `dotnet ef database update --project PriceTrackerCloud.Infrastructure --startup-project PriceTrackerCloud.API`

---

## Convenciones de código

- **CQRS:** MediatR. Command = modifica estado. Query = solo lectura. Un handler por command/query.
- **Validación:** FluentValidation. Un validator por command. El `ValidationBehavior` los ejecuta automáticamente en el pipeline de MediatR antes de llegar al handler.
- **Mappings:** AutoMapper. Solo en `MappingProfile.cs`. Los handlers que devuelven DTOs inyectan `IMapper`.
- **Repositorios:** Acceso siempre a través de `IUnitOfWork`. Nunca inyectar repositorios directamente en handlers.
- **Tests:** xUnit + Moq. Arrange/Act/Assert explícito. Nombres: `Método_Escenario_ResultadoEsperado`.
- **Commits:** Conventional Commits (`feat:`, `fix:`, `chore:`, `test:`, `docs:`).
- **Secretos:** Nunca en código. `appsettings.json` para placeholders, variables de entorno en producción. `.env.example` en frontend.
- **Logging:** Serilog → consola únicamente (compatible con Azure App Service y contenedores Docker).
- **Errores HTTP:** Problem Details (RFC 7807) vía `ErrorHandlingMiddleware`. Nunca lanzar excepciones HTTP directamente desde handlers.
- **UserId en controllers:** siempre extraer del JWT claim `ClaimTypes.NameIdentifier`, nunca como parámetro de la request.

---

## Fases de desarrollo

| Fase | Estado | Resultado |
|------|--------|-----------|
| 0 | ✅ Completada | Esqueleto, CLAUDE.md, .gitignore, solución .sln con 4 proyectos y referencias |
| 1 | ✅ Completada | Entidades Domain, EF Core + Npgsql, Fluent API, repositorios, UoW, migración + seed |
| 2 | ✅ Completada | DTOs, interfaces, Commands/Queries + handlers, validators, AutoMapper, pipeline, 20 tests |
| 3 | ✅ Completada | API: Program.cs, DI completo, JWT, BCrypt, controllers, middleware errores, Swagger, Serilog |
| 4 | **Siguiente** | BackgroundService que revisa precios y dispara alertas (log con Serilog) |
| 5 | Pendiente | Frontend: Vite + React + TS + Tailwind + Redux Toolkit + páginas + conexión API |
| 6 | Pendiente | Dockerfile backend/frontend + docker-compose.yml (3 servicios) |
| 7 | Pendiente | GitHub Actions CI/CD + manifiestos Kubernetes para Azure |
| 8 | Opcional | Elegir: scraper de precios / roles admin completos / Redis cache |

---

## Criterios de aceptación globales
- `dotnet run` (desde `backend/PriceTrackerCloud.API`) arranca el backend sin errores ✅
- Swagger accesible en `/swagger` con botón "Authorize" para JWT ✅
- Login JWT funcionando contra la base de datos real
- `npm run dev` (desde `frontend/`) arranca el frontend
- `docker compose up` levanta los 3 servicios end-to-end
- `dotnet test` → todos los tests en verde ✅ (27/27)

---

## Decisiones técnicas tomadas y su justificación

### 1. MediatR para CQRS (en lugar de implementación manual)
**Decisión:** usar `MediatR 12.4.1`.
**Por qué esta y no otra:** MediatR es el estándar de facto en .NET para CQRS. Aporta el pipeline de behaviors (usado para validación automática con `ValidationBehavior`), desacopla completamente los handlers de los controllers, y es lo que verán en cualquier empresa o código .NET profesional. Una implementación manual añadiría código de fontanería sin valor académico.

### 2. IPasswordHasher e IJwtTokenGenerator como interfaces en Application
**Decisión:** definir las interfaces en Application, implementarlas en Infrastructure.
**Por qué:** el hash de contraseñas y la generación de JWT son detalles de infraestructura (BCrypt, `System.IdentityModel.Tokens.Jwt`). Ponerlos como interfaces en Application permite testear los handlers con Moq sin depender de librerías externas. Es la aplicación directa del principio de inversión de dependencias.

### 3. UnitOfWork como punto de acceso a repositorios
**Decisión:** los handlers solo inyectan `IUnitOfWork`, nunca repositorios individuales.
**Por qué:** garantiza que todos los cambios de una operación se salvan en una única transacción (`SaveChangesAsync` se llama una sola vez). Si inyectáramos repositorios individuales, cada uno tendría su propio DbContext y las transacciones serían inconsistentes.

### 4. Guid como tipo de Id en todas las entidades
**Decisión:** `Guid` en lugar de `int` autoincremental.
**Por qué:** los GUIDs permiten generar el Id en el cliente/aplicación antes de persistir. Con `int` autoincremental el Id solo se conoce tras insertar en base de datos, lo que complica los handlers que devuelven el objeto recién creado.

### 5. HasData con GUIDs fijos para seed
**Decisión:** GUIDs hardcodeados en `SeedData.cs` con patrón `a1a1a1a1-...`.
**Por qué:** EF Core exige que los datos de `HasData` sean completamente deterministas para que las migraciones sean idempotentes. Si se usara `Guid.NewGuid()` en `SeedData`, cada vez que se regenerase la migración los GUIDs cambiarían, corrompiendo los datos existentes en producción.

### 6. ValidationBehavior en el pipeline de MediatR
**Decisión:** validar mediante un `IPipelineBehavior<TRequest, TResponse>` en lugar de validar dentro de cada handler.
**Por qué:** evita duplicar la llamada al validator en cada handler (DRY). El behavior se registra una sola vez y se ejecuta automáticamente para cualquier request que tenga un validator registrado. Si el validator falla, lanza `ValidationException` antes de que el handler llegue a ejecutarse.

### 7. DesignTimeDbContextFactory en Infrastructure
**Decisión:** implementar `IDesignTimeDbContextFactory<PriceTrackerDbContext>` en Infrastructure.
**Por qué:** permite ejecutar `dotnet ef migrations add` sin que PostgreSQL esté corriendo. Sin esta factory, el tooling de EF Core intenta arrancar el proyecto API completo para obtener el DbContext, lo que falla porque no hay base de datos disponible en desarrollo.

### 8. Swashbuckle.AspNetCore 6.9.0 en lugar de Microsoft.AspNetCore.OpenApi nativo
**Decisión:** usar Swashbuckle 6.9.0 y eliminar `Microsoft.AspNetCore.OpenApi` del proyecto API.
**Por qué esta y no otra:** `Microsoft.AspNetCore.OpenApi` (el paquete nativo de .NET 9) usa `Microsoft.OpenApi` 1.x, mientras que Swashbuckle 10.x arrastró `Microsoft.OpenApi` 2.x que tiene breaking changes de namespace (`Microsoft.OpenApi.Models` dejó de existir). La combinación de ambos en el mismo proyecto causa conflictos de versión irresolubles. Swashbuckle 6.9.0 usa `Microsoft.OpenApi` 1.x, es la versión más estable y ampliamente documentada, y tiene soporte completo para la definición de seguridad Bearer que necesitamos para documentar la autenticación JWT en la UI de Swagger.

### 9. Serilog → consola únicamente (sin fichero)
**Decisión:** `Serilog.Sinks.Console` únicamente, sin `Serilog.Sinks.File`.
**Por qué:** el proyecto se desplegará en Azure (App Service o contenedores). En Azure, los ficheros de log locales no persisten entre reinicios del contenedor y no son fácilmente accesibles. Azure captura stdout/stderr automáticamente en Log Stream, por lo que escribir a consola es la estrategia correcta y más sencilla. Añadir un sink de fichero solo aportaría complejidad sin valor en este contexto.

### 10. ErrorHandlingMiddleware captura InvalidOperationException → 409 Conflict
**Decisión:** mapear `InvalidOperationException` a HTTP 409 (además de los tipos del spec original).
**Por qué:** `RegisterUserCommandHandler` lanza `InvalidOperationException` cuando el email ya está registrado. Sin este mapeo, un intento de registro con email duplicado devolvería 500. El 409 Conflict es el código semánticamente correcto para "el recurso ya existe". Se añadió durante la implementación al revisar los handlers existentes.

### 11. System.IdentityModel.Tokens.Jwt añadido explícitamente a Infrastructure
**Decisión:** añadir `System.IdentityModel.Tokens.Jwt 8.0.1` como dependencia directa de Infrastructure.
**Por qué:** `JwtTokenGenerator` vive en Infrastructure y usa `JwtSecurityToken`, `JwtSecurityTokenHandler`, `SigningCredentials`, etc. Aunque `Microsoft.AspNetCore.Authentication.JwtBearer` trae este paquete transitivamente en el proyecto API, Infrastructure no referencia API (violación de la regla de dependencias), así que necesita la referencia directa. Se usa la versión 8.0.1 que es la que trae transitivamente JwtBearer 9.0.5, garantizando compatibilidad.

### 12. Microsoft.Extensions.Configuration.Json añadido a Tests
**Decisión:** añadir `Microsoft.Extensions.Configuration.Json` al proyecto Tests.
**Por qué:** los tests de `JwtTokenGenerator` necesitan construir una `IConfiguration` con `ConfigurationBuilder` + `AddInMemoryCollection()` para inyectar los `JwtSettings` sin depender de archivos de configuración externos. Este paquete no estaba en el proyecto Tests porque en Fases 1-2 no había tests que necesitaran configuración. Se añade en Fase 3 al aparecer el primer test de Infrastructure que necesita configuración.

---

## Problemas encontrados y sus soluciones

### P1 — Warning NU1903: AutoMapper 13.0.1 vulnerabilidad conocida
**Síntoma:** `warning NU1903: Package 'AutoMapper' 13.0.1 has a known high severity vulnerability`.
**Causa:** CVE reportado en AutoMapper 13.x relacionado con deserialización no tipada.
**Solución adoptada:** mantener 13.0.1 por ahora. El CVE afecta a un patrón de uso (`Map` dinámico sin tipo destino) que no empleamos en este proyecto — solo usamos `Map<TDestino>(origen)` con tipos explícitos.
**Qué hacer cuando salga parche:** `dotnet add PriceTrackerCloud.Application package AutoMapper --version X.Y.Z` y verificar que los tests siguen en verde.
**Por qué no cambiar a otra librería:** Mapster sería la alternativa, pero cambiarla a mitad del proyecto rompe los profiles ya escritos. El riesgo real es nulo con nuestro patrón de uso.

### P2 — dotnet-ef no instalado (primer uso de migraciones)
**Síntoma:** `dotnet ef` devuelve "command not found".
**Solución:** `dotnet tool install --global dotnet-ef`. Se instaló la versión 10.0.9.
**Comando para generar migraciones futuras:**
```
dotnet ef migrations add <NombreMigracion> \
  --project PriceTrackerCloud.Infrastructure/PriceTrackerCloud.Infrastructure.csproj \
  --startup-project PriceTrackerCloud.API/PriceTrackerCloud.API.csproj \
  --output-dir Data/Migrations
```
**Comando para aplicar migraciones:**
```
dotnet ef database update \
  --project PriceTrackerCloud.Infrastructure/PriceTrackerCloud.Infrastructure.csproj \
  --startup-project PriceTrackerCloud.API/PriceTrackerCloud.API.csproj
```

### P3 — Seed de ProductPrice con relaciones FK (HasData)
**Síntoma:** al usar `HasData` con entidades que tienen navigation properties (`Product`, `Store`), EF Core lanza error porque los navigation properties no pueden estar poblados en el seed.
**Solución:** en `SeedData.cs` los objetos `ProductPrice` se crean solo con las FK escalares (`ProductId`, `StoreId`), sin asignar las navigation properties. Las navigation properties se cargan en runtime mediante `Include()` en los repositorios.
**Por qué:** `HasData` trabaja directamente con la tabla, no con el grafo de objetos. EF Core requiere que los datos de seed sean "planos" (solo columnas, sin objetos relacionados).

### P4 — Microsoft.AspNetCore.Authentication.JwtBearer 10.x incompatible con .NET 9
**Síntoma:** `error NU1202: Package Microsoft.AspNetCore.Authentication.JwtBearer 10.0.9 is not compatible with net9.0`.
**Causa:** al instalar `JwtBearer` sin especificar versión, NuGet resuelve la última disponible (10.x) que requiere .NET 10.
**Solución:** instalar explícitamente la versión 9.x: `dotnet add ... package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.5`.
**Por qué esta versión:** 9.0.5 es la última versión estable del canal .NET 9, compatible con `net9.0`. La regla general es que los paquetes `Microsoft.AspNetCore.*` deben coincidir con el `TargetFramework` del proyecto.

### P5 — Swashbuckle.AspNetCore 10.x + Microsoft.OpenApi 2.x: namespace Microsoft.OpenApi.Models inexistente
**Síntoma:** `error CS0234: The type or namespace name 'Models' does not exist in the namespace 'Microsoft.OpenApi'`.
**Causa:** Swashbuckle 10.x usa `Microsoft.OpenApi` 2.x como dependencia. En la versión 2.x de `Microsoft.OpenApi`, el namespace `Microsoft.OpenApi.Models` fue reorganizado/eliminado, lo que rompe el código que usa `OpenApiInfo`, `OpenApiSecurityScheme`, etc.
**Solución:** hacer downgrade de Swashbuckle a la versión 6.9.0, que usa `Microsoft.OpenApi` 1.x donde `Microsoft.OpenApi.Models` existe y funciona con normalidad. Además, eliminar `Microsoft.AspNetCore.OpenApi` (el paquete nativo de .NET 9) del `.csproj` del API porque traía una versión diferente de `Microsoft.OpenApi` que conflictuaba.
**Por qué no actualizar el código a la API de Microsoft.OpenApi 2.x:** la migración a 2.x implica cambios en todas las definiciones de Swagger (tipos renombrados, constructores diferentes). Swashbuckle 6.9.0 es la versión más documentada y usada en proyectos .NET 8/9, con abundante documentación y ejemplos. No hay ventaja real en usar la 10.x para este proyecto.

### P6 — System.IdentityModel.Tokens.Jwt no disponible en Infrastructure
**Síntoma:** `error CS0234: The type or namespace name 'IdentityModel' does not exist in the namespace 'System'` al compilar `JwtTokenGenerator.cs`.
**Causa:** `JwtTokenGenerator` usa tipos de `System.IdentityModel.Tokens.Jwt` pero Infrastructure no tiene referencia directa a ese paquete. El paquete solo llegaba transitivamente al proyecto API a través de `JwtBearer`, pero Infrastructure no referencia API (ni debe hacerlo).
**Solución:** añadir `System.IdentityModel.Tokens.Jwt 8.0.1` directamente a Infrastructure: `dotnet add PriceTrackerCloud.Infrastructure package System.IdentityModel.Tokens.Jwt --version 8.0.1`.
**Por qué la versión 8.0.1:** es la versión que trae transitivamente `Microsoft.AspNetCore.Authentication.JwtBearer 9.0.5`. Usar la misma versión garantiza que no haya conflictos de ensamblado entre los dos proyectos en tiempo de ejecución.

### P7 — ConfigurationBuilder no disponible en proyecto Tests
**Síntoma:** `error CS0246: The type or namespace name 'ConfigurationBuilder' could not be found` en `JwtTokenGeneratorTests.cs`.
**Causa:** `ConfigurationBuilder` y `AddInMemoryCollection()` pertenecen a `Microsoft.Extensions.Configuration`, que no estaba referenciado en el proyecto Tests.
**Solución:** `dotnet add PriceTrackerCloud.Tests package Microsoft.Extensions.Configuration.Json`.
**Por qué este paquete y no solo el base:** `Microsoft.Extensions.Configuration.Json` trae transitivamente `Microsoft.Extensions.Configuration` (el base) y `Microsoft.Extensions.Configuration.Abstractions`. Con un solo paquete se cubre todo lo necesario para construir `IConfiguration` en tests. Si solo se añadiera el paquete base, faltarían los extension methods como `AddInMemoryCollection`.

### P8 — Tests no referenciaban Infrastructure
**Síntoma:** `error CS0234: The type or namespace name 'Infrastructure' does not exist in the namespace 'PriceTrackerCloud'` al compilar los nuevos tests de Infrastructure.
**Causa:** el proyecto Tests solo referenciaba Application y Domain (suficiente para las Fases 1-2). Al añadir tests de `BCryptPasswordHasher` y `JwtTokenGenerator` que viven en Infrastructure, el proyecto Tests necesita referencia directa.
**Solución:** `dotnet add PriceTrackerCloud.Tests reference PriceTrackerCloud.Infrastructure`.
**Por qué es aceptable:** en Clean Architecture, los tests de Infrastructure son legítimos (prueban implementaciones concretas como BCrypt o JWT). El proyecto Tests no es parte de la cadena de producción, así que puede referenciar cualquier capa sin violar las reglas de dependencias del código de producción.

---

## Estado actual del build y tests
```
dotnet build PriceTrackerCloud.sln  →  Build succeeded. 0 Error(s)
dotnet test PriceTrackerCloud.sln   →  Total: 27  Passed: 27  Failed: 0
dotnet run (API)                    →  Arranca en http://localhost:5295, Swagger en /swagger
```

**Desglose de tests:**
- Validators/ → 13 tests (Register, CreateProduct, CreateAlert)
- Handlers/ → 7 tests (RegisterUser, GetProducts)
- Infrastructure/ → 7 tests (BCryptPasswordHasher × 4, JwtTokenGenerator × 3)

**Nota para próxima sesión:** para probar los endpoints hace falta PostgreSQL corriendo. Levantar con Docker:
```bash
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=pricetracker postgres:16
dotnet ef database update --project backend/PriceTrackerCloud.Infrastructure --startup-project backend/PriceTrackerCloud.API
dotnet run --project backend/PriceTrackerCloud.API
```
Luego abrir `http://localhost:5295/swagger`.
