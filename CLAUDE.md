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
| Jobs | BackgroundService (`PriceCheckerService`) ✅ |
| Docs | Swashbuckle.AspNetCore 6.9.0 |
| Frontend | React 18 · Vite 8 · TypeScript · Redux Toolkit · RTK Query · TailwindCSS 3 · Recharts 2 |
| HTTP cliente | Axios (custom `axiosBaseQuery` para RTK Query) |
| Routing | React Router **v6.30.4** (importante: NO v7) |
| Tests frontend | Vitest 4 · React Testing Library · jsdom |
| DevOps | Docker · Docker Compose · GitHub Actions · Kubernetes · Azure |
| Tests backend | xUnit + Moq 4.20.72 + FluentAssertions 6.12.1 |

---

## Arquitectura — Clean Architecture

```
proyecto/
├── CLAUDE.md
├── .gitignore
├── docs/
│   └── superpowers/
│       ├── specs/
│       │   ├── 2026-06-14-fase3-api-design.md
│       │   ├── 2026-06-14-fase4-background-service-design.md
│       │   └── 2026-06-14-fase5-frontend-design.md
│       └── plans/
│           ├── 2026-06-14-fase3-api-layer.md
│           ├── 2026-06-14-fase4-background-service.md
│           └── 2026-06-14-fase5-frontend.md
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
│   │   │   ├── BCryptPasswordHasher.cs
│   │   │   └── JwtTokenGenerator.cs
│   │   ├── BackgroundServices/
│   │   │   └── PriceCheckerService.cs   ← Fase 4 ✅
│   │   ├── Data/
│   │   │   ├── PriceTrackerDbContext.cs
│   │   │   ├── DesignTimeDbContextFactory.cs
│   │   │   ├── Configurations/ (5 entidades + SeedData.cs)
│   │   │   └── Migrations/ (InitialCreate — generada)
│   │   ├── Repositories/ (Repository<T>, User, Product, Price, Alert)
│   │   ├── InternalsVisibleTo.cs        ← expone internals a Tests
│   │   ├── UnitOfWork.cs
│   │   └── DependencyInjection.cs
│   ├── PriceTrackerCloud.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── ProductsController.cs
│   │   │   ├── PricesController.cs
│   │   │   └── AlertsController.cs
│   │   ├── Middleware/ErrorHandlingMiddleware.cs
│   │   ├── DependencyInjection.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── PriceTrackerCloud.Tests/
│       ├── Validators/ (13 tests)
│       ├── Handlers/ (7 tests)
│       ├── Infrastructure/ (7 tests)
│       └── BackgroundServices/ (2 tests — PriceCheckerService)
└── frontend/
    ├── package.json
    ├── vite.config.ts
    ├── tsconfig.json + tsconfig.app.json + tsconfig.node.json
    ├── tailwind.config.js · postcss.config.js
    ├── .env.example                      ← VITE_API_URL=http://localhost:5295
    └── src/
        ├── main.tsx · App.tsx · index.css · vite-env.d.ts
        ├── types.ts                       ← interfaces mapeadas de DTOs backend
        ├── test/setup.ts
        ├── api/
        │   ├── axiosBase.ts              ← axios instance + axiosBaseQuery + 401→logout
        │   └── apiSlice.ts               ← RTK Query, 9 endpoints
        ├── store/
        │   ├── authSlice.ts              ← token+user, localStorage seguro
        │   └── store.ts                  ← configureStore + logoutAndReset()
        ├── hooks/useAuth.ts
        └── components/
            ├── ui/ (Spinner, ErrorMessage)
            └── layout/
                ├── ProtectedRoute.tsx    ← redirect a /login?redirect=<ruta>
                ├── ProtectedRoute.test.tsx  ← 2 tests Vitest ✅
                ├── Sidebar.tsx           ← PENDIENTE (Task 6)
                └── Layout.tsx            ← PENDIENTE (Task 6)
```

### Regla de dependencias (backend)
```
API ──► Application ──► Domain
Infrastructure ──► Application
Infrastructure ──► Domain
```
> Domain NO depende de nada externo. Application NO depende de Infrastructure.
> API referencia Infrastructure como Composition Root (DI).

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

## DTOs backend → Tipos frontend

ASP.NET Core serializa a **camelCase** por defecto. Guid → string, decimal → number, DateTime → string ISO 8601.

| DTO backend | Interface TypeScript (`frontend/src/types.ts`) |
|-------------|-----------------------------------------------|
| `AuthResponseDto(Token, Name, Email, Role)` | `AuthResponse { token, name, email, role: string }` |
| `ProductDto(Id, Name, Description, Category)` | `Product { id, name, description, category: string }` |
| `ProductPriceDto(Id, ProductId, StoreName, Price, DateCollected)` | `ProductPrice { id, productId, storeName: string; price: number; dateCollected: string }` |
| `AlertDto(Id, ProductId, ProductName, TargetPrice, IsActive, CreatedAt)` | `Alert { id, productId, productName: string; targetPrice: number; isActive: boolean; createdAt: string }` |

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

---

## Convenciones de código

### Backend
- **CQRS:** MediatR. Command = modifica estado. Query = solo lectura. Un handler por command/query.
- **Validación:** FluentValidation. Un validator por command. `ValidationBehavior` en el pipeline.
- **Mappings:** AutoMapper. Solo en `MappingProfile.cs`.
- **Repositorios:** Acceso siempre a través de `IUnitOfWork`. Nunca inyectar repos directamente.
- **Tests:** xUnit + Moq. Arrange/Act/Assert. Nombres: `Método_Escenario_ResultadoEsperado`.
- **Errores HTTP:** Problem Details (RFC 7807) vía `ErrorHandlingMiddleware`.
- **UserId en controllers:** extraer del JWT claim `ClaimTypes.NameIdentifier`, nunca como parámetro.

### Frontend
- **Estado del servidor:** RTK Query (no thunks manuales). Un `apiSlice` con todos los endpoints.
- **Estado global:** solo `authSlice` en Redux (token + user). Persistido en localStorage con validación.
- **Logout:** siempre usar `logoutAndReset()` de `store.ts` — limpia auth + caché RTK Query.
- **Router:** React Router v6 (NO v7). `ProtectedRoute` usa `<Outlet />` y redirige a `/login?redirect=<ruta>`.
- **Tests frontend:** Vitest + React Testing Library. Mock con `vi.mock` sobre el módulo, no sobre fetch/axios.
- **Commits:** Conventional Commits (`feat:`, `fix:`, `chore:`, `test:`, `docs:`).
- **Secretos:** `.env.example` en frontend (nunca `.env` en git). Variables como `VITE_API_URL`.

---

## Fases de desarrollo

| Fase | Estado | Resultado |
|------|--------|-----------|
| 0 | ✅ Completada | Esqueleto, CLAUDE.md, .gitignore, solución .sln con 4 proyectos |
| 1 | ✅ Completada | Entidades Domain, EF Core + Npgsql, repos, UoW, migración + seed |
| 2 | ✅ Completada | DTOs, Commands/Queries + handlers, validators, AutoMapper, pipeline, 20 tests |
| 3 | ✅ Completada | API: Program.cs, DI, JWT, BCrypt, controllers, middleware, Swagger, Serilog |
| 4 | ✅ Completada | `PriceCheckerService` (BackgroundService): simula precios ±10%, dispara alertas, Serilog. 29/29 tests |
| 5 | 🔄 **En progreso** | Frontend React — Tasks 1-5 completas, Task 6 parcial (ver abajo) |
| 6 | Pendiente | Dockerfile backend/frontend + docker-compose.yml (3 servicios) |
| 7 | Pendiente | GitHub Actions CI/CD + manifiestos Kubernetes para Azure |
| 8 | Opcional | Scraper de precios / roles admin / Redis cache |

---

## Estado de Fase 5 — Frontend (sesión 2026-06-14)

**Plan:** `docs/superpowers/plans/2026-06-14-fase5-frontend.md`

| Task | Estado | Commit |
|------|--------|--------|
| 1 — Scaffold Vite + Tailwind + Vitest | ✅ | `4a6f001`, `7f1ac80`, `ed438b1` |
| 2 — types.ts | ✅ | `1cd155c` |
| 3 — authSlice + store + useAuth | ✅ | `2200612`, `110b339` |
| 4 — axiosBase + apiSlice RTK Query | ✅ | `b1c8b77` |
| 5 — Spinner + ErrorMessage | ✅ | `036d5e8` |
| 6 — ProtectedRoute (TDD) + Sidebar + Layout | 🔄 parcial | ProtectedRoute.tsx + test hechos, **Sidebar.tsx y Layout.tsx PENDIENTES** |
| 7 — App.tsx + main.tsx routing | ⏳ pendiente | — |
| 8 — LoginPage (TDD) | ⏳ pendiente | — |
| 9 — RegisterPage | ⏳ pendiente | — |
| 10 — DashboardPage | ⏳ pendiente | — |
| 11 — ProductsPage | ⏳ pendiente | — |
| 12 — ProductDetailPage (Recharts) | ⏳ pendiente | — |
| 13 — CreateProductPage | ⏳ pendiente | — |
| 14 — AlertsPage | ⏳ pendiente | — |
| 15 — Verificación final | ⏳ pendiente | — |

**Para retomar en la próxima sesión:**
1. Lee este CLAUDE.md para contexto completo
2. El plan de implementación está en `docs/superpowers/plans/2026-06-14-fase5-frontend.md`
3. Continuar desde **Task 6** (completar Sidebar.tsx y Layout.tsx)
4. Usar `superpowers:subagent-driven-development` para ejecutar el plan task por task
5. Antes de ejecutar Task 6, notar que `logoutAndReset()` ya existe en `store.ts` (se añadió como fix de seguridad) — Sidebar debe importarlo de `../../store/store` directamente, sin usar dispatch

---

## Criterios de aceptación globales
- `dotnet run` (desde `backend/PriceTrackerCloud.API`) arranca sin errores ✅
- Swagger en `/swagger` con botón Authorize JWT ✅
- `dotnet test` → 29/29 tests en verde ✅
- `npm run dev` (desde `frontend/`) arranca el frontend ← pendiente completar Fase 5
- `docker compose up` levanta los 3 servicios ← Fase 6
- `dotnet test` frontend → tests en verde ← parcial (2 tests ProtectedRoute escritos pero pendientes de correr)

---

## Estado actual del build y tests
```
Backend:
  dotnet build PriceTrackerCloud.sln  →  Build succeeded. 0 Error(s)
  dotnet test PriceTrackerCloud.sln   →  Total: 29  Passed: 29  Failed: 0
  dotnet run (API)                    →  http://localhost:5295, Swagger en /swagger

Frontend (parcial):
  npm run build   →  0 TypeScript errors, bundle generado ✅
  npm run test    →  2 tests ProtectedRoute escritos, pendiente ejecutar con Sidebar+Layout
  npm run dev     →  arranca en http://localhost:5173 (sin páginas reales aún)
```

**Para levantar el backend completo:**
```bash
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=pricetracker postgres:16
cd backend
dotnet ef database update --project PriceTrackerCloud.Infrastructure --startup-project PriceTrackerCloud.API
dotnet run --project PriceTrackerCloud.API
```

---

## Decisiones técnicas tomadas y su justificación

### 1. MediatR para CQRS
**Decisión:** `MediatR 12.4.1`.
**Por qué:** estándar de facto en .NET para CQRS. Aporta pipeline de behaviors (validación automática), desacopla handlers de controllers. Una implementación manual añadiría código de fontanería sin valor académico.

### 2. IPasswordHasher e IJwtTokenGenerator como interfaces en Application
**Decisión:** interfaces en Application, implementaciones en Infrastructure.
**Por qué:** permite testear handlers con Moq sin depender de BCrypt o JWT. Principio de inversión de dependencias.

### 3. UnitOfWork como punto de acceso a repositorios
**Decisión:** handlers inyectan `IUnitOfWork`, nunca repositorios individuales.
**Por qué:** garantiza que todos los cambios de una operación se salvan en una única transacción (`SaveChangesAsync` una sola vez).

### 4. Guid como tipo de Id
**Decisión:** `Guid` en lugar de `int` autoincremental.
**Por qué:** permite generar el Id en la aplicación antes de persistir. Con `int` el Id solo se conoce tras insertar en BD.

### 5. HasData con GUIDs fijos para seed
**Decisión:** GUIDs hardcodeados en `SeedData.cs` con patrón `a1a1a1a1-...`.
**Por qué:** EF Core requiere datos de `HasData` deterministas para migraciones idempotentes. `Guid.NewGuid()` cambiaría en cada regeneración.

### 6. ValidationBehavior en el pipeline de MediatR
**Decisión:** `IPipelineBehavior<TRequest, TResponse>` en lugar de validar en cada handler.
**Por qué:** evita duplicar la llamada al validator en cada handler (DRY). Se registra una vez y ejecuta automáticamente.

### 7. DesignTimeDbContextFactory en Infrastructure
**Decisión:** `IDesignTimeDbContextFactory<PriceTrackerDbContext>` en Infrastructure.
**Por qué:** permite ejecutar `dotnet ef migrations add` sin PostgreSQL corriendo.

### 8. Swashbuckle 6.9.0 en lugar de Microsoft.AspNetCore.OpenApi nativo
**Decisión:** Swashbuckle 6.9.0, sin `Microsoft.AspNetCore.OpenApi`.
**Por qué:** Swashbuckle 10.x usa `Microsoft.OpenApi` 2.x que eliminó el namespace `Microsoft.OpenApi.Models`. Swashbuckle 6.9.0 usa 1.x donde todo funciona. Los dos paquetes juntos generan conflictos irresolubles.

### 9. Serilog → consola únicamente
**Decisión:** `Serilog.Sinks.Console` sin `Serilog.Sinks.File`.
**Por qué:** en Azure/contenedores los ficheros locales no persisten. Azure captura stdout/stderr en Log Stream automáticamente.

### 10. ErrorHandlingMiddleware captura InvalidOperationException → 409
**Decisión:** mapear `InvalidOperationException` a HTTP 409 Conflict.
**Por qué:** `RegisterUserCommandHandler` lanza esta excepción para email duplicado. Sin el mapeo devolvería 500.

### 11. System.IdentityModel.Tokens.Jwt añadido explícitamente a Infrastructure
**Decisión:** `System.IdentityModel.Tokens.Jwt 8.0.1` referencia directa en Infrastructure.
**Por qué:** `JwtTokenGenerator` vive en Infrastructure. El paquete llegaba transitivamente solo al proyecto API, y Infrastructure no referencia API (violación de reglas de dependencia).

### 12. Microsoft.Extensions.Configuration.Json añadido a Tests
**Decisión:** `Microsoft.Extensions.Configuration.Json` en el proyecto Tests.
**Por qué:** los tests de `JwtTokenGenerator` necesitan `ConfigurationBuilder` + `AddInMemoryCollection()`. Este paquete trae transitivamente el base y las abstractions de una sola vez.

### 13. RTK Query (no thunks manuales) para datos del servidor en frontend
**Decisión:** `createApi` de RTK Query con un único `apiSlice` y 9 endpoints.
**Por qué esta y no otra:** RTK Query es el patrón moderno de Redux Toolkit. Gestiona caché, loading states e invalidación automáticamente. Los thunks con `createAsyncThunk` producen el mismo resultado con 3× más código (hay que escribir los reducers pending/fulfilled/rejected a mano). Estado local con `useState/useEffect` no comparte caché entre páginas.

### 14. axiosBaseQuery custom en lugar de fetchBaseQuery de RTK Query
**Decisión:** implementar un `axiosBaseQuery` propio en `axiosBase.ts` que usa axios.
**Por qué:** el interceptor de 401 necesita acceder al store para despachar `logout()`. Con `fetchBaseQuery` nativo de RTK Query esto es más difícil de encapsular. Con axios la lógica queda en un único lugar: si el backend devuelve 401, despacha logout automáticamente. Única complejidad: el import circular `axiosBase → store → apiSlice → axiosBase` se resuelve porque el import de `RootState` en axiosBase.ts es `import type` (solo tipos, borrados en runtime).

### 15. logoutAndReset() como función standalone en store.ts
**Decisión:** exportar `logoutAndReset()` desde `store.ts` que llama tanto a `logout()` como a `apiSlice.util.resetApiState()`.
**Por qué:** sin resetear el caché de RTK Query al hacer logout, los datos del usuario anterior permanecen en memoria y se muestran al siguiente usuario que inicia sesión. La alternativa (dispatch dentro de un thunk) requiere tipado extra. La función standalone con acceso al singleton del store es la opción más simple. Toda la lógica de logout queda en un punto. Sidebar.tsx llama a `logoutAndReset()` directamente, no a través de dispatch.

### 16. localStorage con validación defensiva para persistir auth
**Decisión:** `loadAuthState()` con try/catch y validación de tipos antes de deserializar.
**Por qué:** `JSON.parse()` sin try/catch lanza excepción si localStorage contiene JSON corrupto (puede pasar si el usuario edita manualmente o hay un error de escritura previo). Además, se valida que el objeto parseado tenga la forma esperada (`token: string`, `user.role: string`) — un usuario malintencionado podría editar localStorage para cambiar su rol, pero el backend siempre valida el JWT, así que el daño es solo de UI. La validación de tipos minimiza el riesgo.

### 17. React Router v6 fijado en 6.30.4 (NO v7)
**Decisión:** `"react-router-dom": "6.30.4"` (pin exacto, no `^7`).
**Por qué:** npm instala la versión más reciente si no se especifica, que en este momento es v7. React Router v7 introdujo un modo "framework" (tipo Remix) que cambia el API de routing. Aunque el modo "library" de v7 mantiene compatibilidad parcial, la documentación, los ejemplos y la configuración de v7 son diferentes. El plan usa `<BrowserRouter>`, `<Routes>`, `<Route>`, `<NavLink>`, `useNavigate`, `useParams` — todo API de v6. Usar v6 garantiza que cualquier documentación consultada es aplicable.

### 18. vite.config.ts importa de `vitest/config` no de `vite`
**Decisión:** `import { defineConfig } from 'vitest/config'` en `vite.config.ts`.
**Por qué:** en Vitest 4.x, importar `defineConfig` de `vite` hace que TypeScript no reconozca el campo `test` del config y genera errores de tipo. `vitest/config` re-exporta `defineConfig` de Vite pero con los tipos de Vitest combinados. Sin esta distinción, el proyecto compila pero el editor muestra errores de tipo en vite.config.ts.

### 19. "types": ["vitest/globals"] en tsconfig.app.json
**Decisión:** añadir `"types": ["vitest/globals"]` a `compilerOptions` de `tsconfig.app.json`.
**Por qué:** con `globals: true` en vite.config.ts, Vitest inyecta `describe`, `it`, `test`, `expect`, `vi` como globales. Sin declarar los tipos en tsconfig, `tsc -b` (que corre durante `npm run build`) no reconoce esas variables en los archivos de test y genera `error TS2304: Cannot find name 'describe'`. Este error bloquearía el build en CI.

### 20. VITE_API_URL tipado en vite-env.d.ts
**Decisión:** declarar `ImportMetaEnv` con `VITE_API_URL: string` en `src/vite-env.d.ts`.
**Por qué:** sin esta declaración, `import.meta.env.VITE_API_URL` tiene tipo `any`. TypeScript no puede detectar si se usa mal (typos en el nombre, undefined en producción). Con la declaración explícita, el tipo es `string` y el compilador avisa si la variable no existe.

---

## Problemas encontrados y sus soluciones

### P1 — Warning NU1903: AutoMapper 13.0.1 vulnerabilidad conocida
**Síntoma:** `warning NU1903: Package 'AutoMapper' 13.0.1 has a known high severity vulnerability`.
**Causa:** CVE en AutoMapper 13.x con deserialización no tipada.
**Solución:** mantener 13.0.1. El CVE afecta a `Map` dinámico sin tipo destino; este proyecto solo usa `Map<TDestino>(origen)` con tipos explícitos. Riesgo real: nulo.

### P2 — dotnet-ef no instalado
**Síntoma:** `dotnet ef` devuelve "command not found".
**Solución:** `dotnet tool install --global dotnet-ef` (versión 10.0.9).
```bash
# Generar migración:
dotnet ef migrations add <Nombre> --project PriceTrackerCloud.Infrastructure/... --startup-project PriceTrackerCloud.API/...
# Aplicar:
dotnet ef database update --project PriceTrackerCloud.Infrastructure/... --startup-project PriceTrackerCloud.API/...
```

### P3 — Seed de ProductPrice con navigation properties en HasData
**Síntoma:** EF Core lanza error al usar `HasData` con objetos que tienen navigation properties.
**Causa:** `HasData` trabaja directamente con la tabla, no con el grafo de objetos.
**Solución:** crear `ProductPrice` solo con FKs escalares (`ProductId`, `StoreId`), sin asignar navigation properties.

### P4 — JwtBearer 10.x incompatible con .NET 9
**Síntoma:** `error NU1202: Package ... 10.0.9 is not compatible with net9.0`.
**Causa:** NuGet resuelve la última versión (10.x que requiere .NET 10) si no se especifica versión.
**Solución:** `dotnet add ... package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.5`.

### P5 — Swashbuckle 10.x + Microsoft.OpenApi 2.x: namespace inexistente
**Síntoma:** `error CS0234: 'Models' does not exist in namespace 'Microsoft.OpenApi'`.
**Causa:** Swashbuckle 10.x usa Microsoft.OpenApi 2.x que reorganizó/eliminó `Microsoft.OpenApi.Models`.
**Solución:** downgrade a Swashbuckle 6.9.0 + eliminar `Microsoft.AspNetCore.OpenApi` del .csproj.

### P6 — System.IdentityModel.Tokens.Jwt no disponible en Infrastructure
**Síntoma:** `error CS0234: 'IdentityModel' does not exist in namespace 'System'`.
**Causa:** el paquete llegaba transitivamente solo al API, que Infrastructure no puede referenciar.
**Solución:** `dotnet add PriceTrackerCloud.Infrastructure package System.IdentityModel.Tokens.Jwt --version 8.0.1`.

### P7 — ConfigurationBuilder no disponible en Tests
**Síntoma:** `error CS0246: 'ConfigurationBuilder' could not be found`.
**Solución:** `dotnet add PriceTrackerCloud.Tests package Microsoft.Extensions.Configuration.Json`.

### P8 — Tests no referenciaban Infrastructure
**Síntoma:** `error CS0234: 'Infrastructure' does not exist in namespace 'PriceTrackerCloud'`.
**Solución:** `dotnet add PriceTrackerCloud.Tests reference PriceTrackerCloud.Infrastructure`.

### P9 — npm instala react-router-dom v7 en lugar de v6
**Síntoma (detectado en code review):** `npm install react-router-dom` instaló v7.6.2. El plan usa API de v6.
**Causa:** npm sin versión especificada instala la última disponible.
**Solución:** `npm install react-router-dom@^6.28.0` → instaló 6.30.4.
**Por qué no quedarse en v7:** el API de routing (BrowserRouter, Routes, Route, NavLink, useNavigate, useParams, Outlet) está especificado en el plan con semántica v6. Usar v7 requeriría revisar cada uso y posiblemente cambiar a framework mode. No hay beneficio para un TFG.

### P10 — TypeScript no reconoce globals de Vitest en build
**Síntoma (detectado en code review):** `tsc -b` genera `error TS2304: Cannot find name 'describe'` en archivos de test.
**Causa:** `globals: true` en vite.config.ts inyecta globales en runtime pero TypeScript no sabe de ellos sin una declaración de tipos.
**Solución:** añadir `"types": ["vitest/globals"]` a `compilerOptions` en `tsconfig.app.json`.

### P11 — vite.config.ts con import de `vite` en lugar de `vitest/config`
**Síntoma:** errores de tipo en `vite.config.ts` — TypeScript no reconoce el campo `test`.
**Causa:** en Vitest 4.x, `defineConfig` de `vite` no incluye los tipos del bloque `test`.
**Solución:** cambiar `import { defineConfig } from 'vite'` → `import { defineConfig } from 'vitest/config'`.

### P12 — JSON.parse(localStorage) sin try/catch
**Síntoma (security review):** `JSON.parse` lanza excepción si localStorage contiene JSON malformado, rompiendo la app al cargar.
**Causa:** localStorage puede corromperse por errores previos o edición manual del usuario.
**Solución:** envolver en `loadAuthState()` con try/catch + validación de tipos. Si falla, limpia localStorage y devuelve estado vacío.

### P13 — Caché RTK Query no se limpia al hacer logout
**Síntoma (security review):** al cerrar sesión con `dispatch(logout())`, los datos del usuario anterior permanecen en el caché de RTK Query y se muestran si un segundo usuario inicia sesión en la misma pestaña.
**Causa:** `logout()` solo limpia el slice de auth, no el estado de la API.
**Solución:** crear `logoutAndReset()` en `store.ts` que despacha tanto `logout()` como `apiSlice.util.resetApiState()`. Toda la UI usa `logoutAndReset()` en lugar de `dispatch(logout())`.

---

## Archivos de configuración importantes

### frontend/vite.config.ts
```ts
import { defineConfig } from 'vitest/config'  // ← vitest/config, NO vite
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts',
  },
})
```

### frontend/tsconfig.app.json (compilerOptions clave)
```json
{
  "compilerOptions": {
    "types": ["vitest/globals"],
    "strict": true,
    "target": "ES2020",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "jsx": "react-jsx"
  }
}
```

### frontend/src/store/store.ts (función clave)
```ts
export function logoutAndReset() {
  store.dispatch(logout())
  store.dispatch(apiSlice.util.resetApiState())
  localStorage.removeItem('auth')
}
```
