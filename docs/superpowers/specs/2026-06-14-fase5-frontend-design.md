# Fase 5 — Frontend React

**Fecha:** 2026-06-14  
**Estado:** Aprobado  
**Proyecto:** PriceTracker Cloud (TFG)

---

## Objetivo

Implementar el frontend completo de PriceTracker Cloud conectado a la API existente. El usuario puede autenticarse, consultar productos con su histórico de precios, gestionar alertas de bajada y ver un dashboard de resumen.

---

## Stack

| Tecnología | Versión / rol |
|---|---|
| React | 18 — UI |
| Vite | Bundler + dev server |
| TypeScript | Tipado estático |
| Redux Toolkit | Estado global (authSlice) + RTK Query (datos del servidor) |
| TailwindCSS | Estilos |
| Recharts | Gráfica de histórico de precios |
| Axios | Cliente HTTP con interceptor JWT |
| React Router v6 | Routing + rutas protegidas |
| Vitest + React Testing Library | 2 tests (ProtectedRoute + LoginPage) |

---

## Estructura de archivos

```
frontend/
├── index.html
├── vite.config.ts
├── tsconfig.json
├── tailwind.config.js
├── .env.example              ← VITE_API_URL=http://localhost:5295
└── src/
    ├── main.tsx
    ├── App.tsx               ← Router + rutas protegidas
    ├── api/
    │   ├── axiosBase.ts      ← instancia Axios con baseURL + interceptor JWT
    │   └── apiSlice.ts       ← RTK Query createApi (todos los endpoints)
    ├── store/
    │   ├── store.ts          ← configureStore
    │   └── authSlice.ts      ← token + user, persistido en localStorage
    ├── hooks/
    │   └── useAuth.ts        ← selector de authSlice + helpers
    ├── components/
    │   ├── layout/
    │   │   ├── Sidebar.tsx
    │   │   ├── Layout.tsx    ← Sidebar + <Outlet />
    │   │   └── ProtectedRoute.tsx
    │   └── ui/
    │       ├── Spinner.tsx
    │       └── ErrorMessage.tsx
    └── pages/
        ├── auth/
        │   ├── LoginPage.tsx
        │   └── RegisterPage.tsx
        ├── DashboardPage.tsx
        ├── products/
        │   ├── ProductsPage.tsx
        │   ├── ProductDetailPage.tsx
        │   └── CreateProductPage.tsx
        └── alerts/
            └── AlertsPage.tsx
```

---

## Rutas

| Ruta | Componente | Auth |
|---|---|---|
| `/login` | LoginPage | Pública |
| `/register` | RegisterPage | Pública |
| `/dashboard` | DashboardPage | Protegida |
| `/products` | ProductsPage | Protegida |
| `/products/new` | CreateProductPage | Protegida |
| `/products/:id` | ProductDetailPage | Protegida |
| `/alerts` | AlertsPage | Protegida |
| `/` | Redirect a `/dashboard` | — |

`ProtectedRoute` comprueba si hay token en el store. Si no hay token, redirige a `/login?redirect=/ruta-original`. Tras login exitoso, lee `?redirect` y navega a la URL guardada (o `/dashboard` por defecto).

---

## Estado global

### `authSlice`

```ts
interface AuthState {
  token: string | null
  user: { id: string; name: string; email: string } | null
}
```

- `setCredentials({ token, user })` — se llama tras login/register exitoso
- `logout()` — limpia store y localStorage
- Persistido en `localStorage` bajo la clave `auth`

### `apiSlice` (RTK Query)

Un único `createApi` con `baseQuery` sobre la instancia Axios de `axiosBase.ts`.

| Endpoint | Tipo | Ruta backend | Tags |
|---|---|---|---|
| `login` | mutation | POST /auth/login | — |
| `register` | mutation | POST /auth/register | — |
| `getProducts` | query | GET /products | `Products` |
| `getProductById` | query | GET /products/{id} | `Products` |
| `createProduct` | mutation | POST /products | invalida `Products` |
| `getPriceHistory` | query | GET /prices/history/{productId} | `Prices` |
| `getAlerts` | query | GET /alerts | `Alerts` |
| `createAlert` | mutation | POST /alerts | invalida `Alerts` |
| `deleteAlert` | mutation | DELETE /alerts/{id} | invalida `Alerts` |

### `axiosBase.ts`

- `baseURL` leída de `import.meta.env.VITE_API_URL`
- Interceptor de request: añade `Authorization: Bearer <token>` leyendo del store
- Interceptor de response: si recibe 401, despacha `logout()` y redirige a `/login`

---

## Páginas

### LoginPage / RegisterPage

- Formulario centrado, sin sidebar
- Validación en cliente: campos no vacíos, email válido
- Tras éxito: `setCredentials` + redirect a `?redirect` o `/dashboard`
- Muestra error del backend (Problem Details) bajo el formulario

### DashboardPage

Tres tarjetas de resumen calculadas en cliente con datos ya en caché (sin llamadas extra):
- **Total productos** — longitud de `getProducts`
- **Alertas activas** — `getAlerts` filtrado por `isActive === true`
- **Alertas disparadas** — `getAlerts` filtrado por `isActive === false` (alertas cuyo precio objetivo ya se alcanzó)

### ProductsPage

- Grid/tabla de productos
- Input de búsqueda que filtra en cliente por nombre/categoría
- Botón "Nuevo producto" → `/products/new`
- Cada fila clicable → `/products/:id`

### ProductDetailPage

- Cabecera: nombre y categoría del producto
- `LineChart` de Recharts con el histórico de precios
  - Eje X: fecha (`DateCollected`)
  - Eje Y: precio
  - Una línea por tienda (colores distintos), con leyenda
- Botón "Crear alerta" que abre un formulario inline con el `TargetPrice`

### CreateProductPage

- Formulario: Nombre, Descripción, Categoría
- Al guardar: llama a `createProduct`, invalida caché, redirige a `/products`

### AlertsPage

- Lista de alertas: nombre del producto, precio objetivo, badge activa/inactiva
- Botón eliminar por alerta (con confirmación inline)
- Formulario de creación: selector de producto (dropdown de `getProducts`) + campo precio objetivo

---

## Manejo de errores y estados de carga

- `isLoading` en queries → `<Spinner>` centrado
- `isError` en queries → `<ErrorMessage message={error}>` 
- Botones de submit: spinner inline + `disabled` mientras mutation `isLoading`
- 401 del backend → interceptor Axios despacha `logout()` + redirect a `/login`
- Errores de mutación → mensaje inline en el formulario (campo `data.detail` de Problem Details)

---

## Tests

**Herramienta:** Vitest + React Testing Library (integrados con Vite)

| Test | Archivo | Qué verifica |
|---|---|---|
| `ProtectedRoute` sin token redirige a `/login` | `src/components/layout/ProtectedRoute.test.tsx` | Sin token en store → URL cambia a `/login` |
| `LoginPage` submit llama al endpoint y guarda token | `src/pages/auth/LoginPage.test.tsx` | Mock del endpoint → `setCredentials` llamado → redirect |

---

## Criterios de aceptación

- `npm run dev` arranca sin errores en `http://localhost:5173`
- Login/Register funciona contra la API real (con PostgreSQL levantado)
- Rutas protegidas redirigen a `/login` sin token, y vuelven a la URL original tras autenticarse
- Dashboard muestra las tres tarjetas con datos reales
- Gráfica de precios renderiza correctamente con los datos seed de la BD
- Crear producto y crear alerta funcionan y actualizan las listas
- Eliminar alerta funciona
- `npm run test` → 2 tests en verde
