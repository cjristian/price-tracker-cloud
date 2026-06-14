# Fase 5 — Frontend React Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implementar el frontend completo de PriceTracker Cloud (React 18 + Vite + TypeScript + Redux Toolkit + RTK Query + TailwindCSS + Recharts) conectado a la API .NET 9 existente.

**Architecture:** RTK Query gestiona todos los datos del servidor con caché automática; un `authSlice` persiste el token JWT en localStorage. El layout con sidebar envuelve todas las rutas protegidas. `ProtectedRoute` redirige a `/login?redirect=<ruta>` si no hay token.

**Tech Stack:** React 18 · Vite · TypeScript · Redux Toolkit + RTK Query · TailwindCSS · Recharts · Axios (baseQuery) · React Router v6 · Vitest + React Testing Library

---

## Tipos del backend (referencia)

Estos son los shapes exactos que devuelve la API (camelCase por defecto en ASP.NET Core):

```ts
// POST /auth/login, POST /auth/register
interface AuthResponse { token: string; name: string; email: string; role: string }

// GET /products, GET /products/:id
interface Product { id: string; name: string; description: string; category: string }

// GET /prices/history/:productId
interface ProductPrice { id: string; productId: string; storeName: string; price: number; dateCollected: string }

// GET /alerts
interface Alert { id: string; productId: string; productName: string; targetPrice: number; isActive: boolean; createdAt: string }
```

---

## File Map

| Acción | Archivo |
|--------|---------|
| Create | `frontend/index.html` |
| Create | `frontend/vite.config.ts` |
| Create | `frontend/tsconfig.json` |
| Create | `frontend/tsconfig.node.json` |
| Create | `frontend/tailwind.config.js` |
| Create | `frontend/postcss.config.js` |
| Create | `frontend/.env.example` |
| Create | `frontend/src/main.tsx` |
| Create | `frontend/src/index.css` |
| Create | `frontend/src/App.tsx` |
| Create | `frontend/src/types.ts` |
| Create | `frontend/src/store/authSlice.ts` |
| Create | `frontend/src/store/store.ts` |
| Create | `frontend/src/hooks/useAuth.ts` |
| Create | `frontend/src/api/axiosBase.ts` |
| Create | `frontend/src/api/apiSlice.ts` |
| Create | `frontend/src/components/ui/Spinner.tsx` |
| Create | `frontend/src/components/ui/ErrorMessage.tsx` |
| Create | `frontend/src/components/layout/Sidebar.tsx` |
| Create | `frontend/src/components/layout/Layout.tsx` |
| Create | `frontend/src/components/layout/ProtectedRoute.tsx` |
| Create | `frontend/src/test/setup.ts` |
| Create | `frontend/src/components/layout/ProtectedRoute.test.tsx` |
| Create | `frontend/src/pages/auth/LoginPage.tsx` |
| Create | `frontend/src/pages/auth/LoginPage.test.tsx` |
| Create | `frontend/src/pages/auth/RegisterPage.tsx` |
| Create | `frontend/src/pages/DashboardPage.tsx` |
| Create | `frontend/src/pages/products/ProductsPage.tsx` |
| Create | `frontend/src/pages/products/ProductDetailPage.tsx` |
| Create | `frontend/src/pages/products/CreateProductPage.tsx` |
| Create | `frontend/src/pages/alerts/AlertsPage.tsx` |

---

## Task 1: Scaffold del proyecto Vite + dependencias + Tailwind + Vitest

**Files:**
- Create: `frontend/vite.config.ts`
- Create: `frontend/tsconfig.json`
- Create: `frontend/tsconfig.node.json`
- Create: `frontend/tailwind.config.js`
- Create: `frontend/postcss.config.js`
- Create: `frontend/.env.example`
- Create: `frontend/src/index.css`

- [ ] **Step 1: Inicializar el proyecto Vite dentro de `frontend/`**

Desde `c:\Users\crich\Desktop\proyecto\frontend\` (o desde `proyecto\` con el flag `--outDir`):

```bash
cd frontend
npm create vite@latest . -- --template react-ts
```

Cuando pregunte si continuar en directorio no vacío, responde `y`. Elimina el `.gitkeep` si lo pide.

- [ ] **Step 2: Instalar dependencias de producción**

```bash
npm install react-router-dom @reduxjs/toolkit react-redux axios recharts
```

- [ ] **Step 3: Instalar dependencias de desarrollo**

```bash
npm install -D tailwindcss postcss autoprefixer vitest @testing-library/react @testing-library/jest-dom @testing-library/user-event jsdom
```

- [ ] **Step 4: Inicializar Tailwind**

```bash
npx tailwindcss init -p
```

- [ ] **Step 5: Reemplazar `tailwind.config.js`**

```js
/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: { extend: {} },
  plugins: [],
}
```

- [ ] **Step 6: Reemplazar `src/index.css`**

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

- [ ] **Step 7: Reemplazar `vite.config.ts`**

```ts
import { defineConfig } from 'vite'
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

- [ ] **Step 8: Crear `src/test/setup.ts`**

```ts
import '@testing-library/jest-dom'
```

- [ ] **Step 9: Crear `.env.example`**

```
VITE_API_URL=http://localhost:5295
```

- [ ] **Step 10: Añadir script de test en `package.json`**

Añadir dentro de `"scripts"`:
```json
"test": "vitest run",
"test:watch": "vitest"
```

- [ ] **Step 11: Verificar que el proyecto arranca**

```bash
npm run dev
```

Resultado esperado: servidor en `http://localhost:5173` sin errores de compilación.

- [ ] **Step 12: Commit**

```bash
cd ..
git add frontend/
git commit -m "chore: scaffold Vite + React + TS + Tailwind + Vitest"
```

---

## Task 2: Tipos compartidos

**Files:**
- Create: `frontend/src/types.ts`

- [ ] **Step 1: Crear `src/types.ts`**

```ts
export interface AuthResponse {
  token: string
  name: string
  email: string
  role: string
}

export interface Product {
  id: string
  name: string
  description: string
  category: string
}

export interface ProductPrice {
  id: string
  productId: string
  storeName: string
  price: number
  dateCollected: string
}

export interface Alert {
  id: string
  productId: string
  productName: string
  targetPrice: number
  isActive: boolean
  createdAt: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  name: string
  email: string
  password: string
}

export interface CreateProductRequest {
  name: string
  description: string
  category: string
}

export interface CreateAlertRequest {
  productId: string
  targetPrice: number
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/types.ts
git commit -m "chore: add shared TypeScript types"
```

---

## Task 3: Store — authSlice + store

**Files:**
- Create: `frontend/src/store/authSlice.ts`
- Create: `frontend/src/store/store.ts`
- Create: `frontend/src/hooks/useAuth.ts`

- [ ] **Step 1: Crear `src/store/authSlice.ts`**

```ts
import { createSlice, type PayloadAction } from '@reduxjs/toolkit'

interface AuthUser {
  name: string
  email: string
  role: string
}

interface AuthState {
  token: string | null
  user: AuthUser | null
}

const stored = localStorage.getItem('auth')
const initialState: AuthState = stored ? JSON.parse(stored) : { token: null, user: null }

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials(state, action: PayloadAction<{ token: string; user: AuthUser }>) {
      state.token = action.payload.token
      state.user = action.payload.user
      localStorage.setItem('auth', JSON.stringify({ token: state.token, user: state.user }))
    },
    logout(state) {
      state.token = null
      state.user = null
      localStorage.removeItem('auth')
    },
  },
})

export const { setCredentials, logout } = authSlice.actions
export default authSlice.reducer
```

- [ ] **Step 2: Crear `src/store/store.ts`**

```ts
import { configureStore } from '@reduxjs/toolkit'
import authReducer from './authSlice'
import { apiSlice } from '../api/apiSlice'

export const store = configureStore({
  reducer: {
    auth: authReducer,
    [apiSlice.reducerPath]: apiSlice.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(apiSlice.middleware),
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
```

- [ ] **Step 3: Crear `src/hooks/useAuth.ts`**

```ts
import { useSelector } from 'react-redux'
import type { RootState } from '../store/store'

export function useAuth() {
  const token = useSelector((state: RootState) => state.auth.token)
  const user = useSelector((state: RootState) => state.auth.user)
  return { token, user, isAuthenticated: token !== null }
}
```

---

## Task 4: API layer — axiosBase + apiSlice

**Files:**
- Create: `frontend/src/api/axiosBase.ts`
- Create: `frontend/src/api/apiSlice.ts`

- [ ] **Step 1: Crear `src/api/axiosBase.ts`**

```ts
import axios from 'axios'
import type { BaseQueryFn } from '@reduxjs/toolkit/query'
import type { AxiosRequestConfig, AxiosError } from 'axios'
import { logout } from '../store/authSlice'
import type { RootState } from '../store/store'

export const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5295',
})

type AxiosArgs = {
  url: string
  method?: AxiosRequestConfig['method']
  data?: unknown
  params?: unknown
}

export const axiosBaseQuery =
  (): BaseQueryFn<AxiosArgs, unknown, { status?: number; data: unknown }> =>
  async ({ url, method = 'GET', data, params }, api) => {
    const token = (api.getState() as RootState).auth.token
    try {
      const result = await axiosInstance({
        url,
        method,
        data,
        params,
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      })
      return { data: result.data }
    } catch (err) {
      const axiosErr = err as AxiosError
      if (axiosErr.response?.status === 401) {
        api.dispatch(logout())
      }
      return {
        error: {
          status: axiosErr.response?.status,
          data: axiosErr.response?.data ?? axiosErr.message,
        },
      }
    }
  }
```

- [ ] **Step 2: Crear `src/api/apiSlice.ts`**

```ts
import { createApi } from '@reduxjs/toolkit/query/react'
import { axiosBaseQuery } from './axiosBase'
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  Product,
  CreateProductRequest,
  ProductPrice,
  Alert,
  CreateAlertRequest,
} from '../types'

export const apiSlice = createApi({
  reducerPath: 'api',
  baseQuery: axiosBaseQuery(),
  tagTypes: ['Products', 'Alerts', 'Prices'],
  endpoints: (builder) => ({
    login: builder.mutation<AuthResponse, LoginRequest>({
      query: (body) => ({ url: '/auth/login', method: 'POST', data: body }),
    }),
    register: builder.mutation<AuthResponse, RegisterRequest>({
      query: (body) => ({ url: '/auth/register', method: 'POST', data: body }),
    }),
    getProducts: builder.query<Product[], void>({
      query: () => ({ url: '/products' }),
      providesTags: ['Products'],
    }),
    getProductById: builder.query<Product, string>({
      query: (id) => ({ url: `/products/${id}` }),
      providesTags: ['Products'],
    }),
    createProduct: builder.mutation<Product, CreateProductRequest>({
      query: (body) => ({ url: '/products', method: 'POST', data: body }),
      invalidatesTags: ['Products'],
    }),
    getPriceHistory: builder.query<ProductPrice[], string>({
      query: (productId) => ({ url: `/prices/history/${productId}` }),
      providesTags: ['Prices'],
    }),
    getAlerts: builder.query<Alert[], void>({
      query: () => ({ url: '/alerts' }),
      providesTags: ['Alerts'],
    }),
    createAlert: builder.mutation<Alert, CreateAlertRequest>({
      query: (body) => ({ url: '/alerts', method: 'POST', data: body }),
      invalidatesTags: ['Alerts'],
    }),
    deleteAlert: builder.mutation<void, string>({
      query: (id) => ({ url: `/alerts/${id}`, method: 'DELETE' }),
      invalidatesTags: ['Alerts'],
    }),
  }),
})

export const {
  useLoginMutation,
  useRegisterMutation,
  useGetProductsQuery,
  useGetProductByIdQuery,
  useCreateProductMutation,
  useGetPriceHistoryQuery,
  useGetAlertsQuery,
  useCreateAlertMutation,
  useDeleteAlertMutation,
} = apiSlice
```

- [ ] **Step 3: Commit**

```bash
git add frontend/src/
git commit -m "feat: add Redux store, authSlice, RTK Query apiSlice"
```

---

## Task 5: Componentes UI base

**Files:**
- Create: `frontend/src/components/ui/Spinner.tsx`
- Create: `frontend/src/components/ui/ErrorMessage.tsx`

- [ ] **Step 1: Crear `src/components/ui/Spinner.tsx`**

```tsx
export function Spinner() {
  return (
    <div className="flex justify-center items-center p-8">
      <div className="w-8 h-8 border-4 border-blue-500 border-t-transparent rounded-full animate-spin" />
    </div>
  )
}
```

- [ ] **Step 2: Crear `src/components/ui/ErrorMessage.tsx`**

```tsx
interface Props {
  message?: string
}

export function ErrorMessage({ message = 'Ha ocurrido un error.' }: Props) {
  return (
    <div className="bg-red-50 border border-red-300 text-red-700 rounded p-4 text-sm">
      {message}
    </div>
  )
}
```

---

## Task 6: Layout + ProtectedRoute (TDD)

**Files:**
- Create: `frontend/src/components/layout/ProtectedRoute.tsx`
- Create: `frontend/src/components/layout/ProtectedRoute.test.tsx`
- Create: `frontend/src/components/layout/Sidebar.tsx`
- Create: `frontend/src/components/layout/Layout.tsx`

- [ ] **Step 1: Escribir el test en rojo para `ProtectedRoute`**

Crear `src/components/layout/ProtectedRoute.test.tsx`:

```tsx
import { render, screen } from '@testing-library/react'
import { MemoryRouter, Routes, Route } from 'react-router-dom'
import { Provider } from 'react-redux'
import { configureStore } from '@reduxjs/toolkit'
import authReducer from '../../store/authSlice'
import { apiSlice } from '../../api/apiSlice'
import { ProtectedRoute } from './ProtectedRoute'

function makeStore(token: string | null) {
  return configureStore({
    reducer: { auth: authReducer, [apiSlice.reducerPath]: apiSlice.reducer },
    middleware: (g) => g().concat(apiSlice.middleware),
    preloadedState: { auth: { token, user: token ? { name: 'Test', email: 'test@test.com', role: 'User' } : null } },
  })
}

test('redirects to /login when not authenticated', () => {
  render(
    <Provider store={makeStore(null)}>
      <MemoryRouter initialEntries={['/dashboard']}>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<div>Dashboard</div>} />
          </Route>
          <Route path="/login" element={<div>Login page</div>} />
        </Routes>
      </MemoryRouter>
    </Provider>
  )
  expect(screen.getByText('Login page')).toBeInTheDocument()
  expect(screen.queryByText('Dashboard')).not.toBeInTheDocument()
})

test('renders children when authenticated', () => {
  render(
    <Provider store={makeStore('fake-token')}>
      <MemoryRouter initialEntries={['/dashboard']}>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<div>Dashboard</div>} />
          </Route>
          <Route path="/login" element={<div>Login page</div>} />
        </Routes>
      </MemoryRouter>
    </Provider>
  )
  expect(screen.getByText('Dashboard')).toBeInTheDocument()
})
```

- [ ] **Step 2: Ejecutar los tests y verificar que fallan**

```bash
cd frontend
npm run test
```

Resultado esperado: FAIL — `Cannot find module './ProtectedRoute'`.

- [ ] **Step 3: Crear `src/components/layout/ProtectedRoute.tsx`**

```tsx
import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'

export function ProtectedRoute() {
  const { isAuthenticated } = useAuth()
  const location = useLocation()

  if (!isAuthenticated) {
    return <Navigate to={`/login?redirect=${location.pathname}`} replace />
  }

  return <Outlet />
}
```

- [ ] **Step 4: Ejecutar los tests y verificar que pasan**

```bash
npm run test
```

Resultado esperado:
```
✓ redirects to /login when not authenticated
✓ renders children when authenticated
```

- [ ] **Step 5: Crear `src/components/layout/Sidebar.tsx`**

```tsx
import { NavLink, useNavigate } from 'react-router-dom'
import { useDispatch } from 'react-redux'
import { logout } from '../../store/authSlice'
import { useAuth } from '../../hooks/useAuth'

const links = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/products', label: 'Productos' },
  { to: '/alerts', label: 'Alertas' },
]

export function Sidebar() {
  const dispatch = useDispatch()
  const navigate = useNavigate()
  const { user } = useAuth()

  function handleLogout() {
    dispatch(logout())
    navigate('/login')
  }

  return (
    <aside className="w-56 min-h-screen bg-gray-900 text-white flex flex-col">
      <div className="px-6 py-5 text-xl font-bold border-b border-gray-700">
        PriceTracker
      </div>
      <nav className="flex-1 px-4 py-6 space-y-1">
        {links.map(({ to, label }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              `block px-4 py-2 rounded text-sm font-medium transition-colors ${
                isActive
                  ? 'bg-blue-600 text-white'
                  : 'text-gray-300 hover:bg-gray-700'
              }`
            }
          >
            {label}
          </NavLink>
        ))}
      </nav>
      <div className="px-6 py-4 border-t border-gray-700">
        <p className="text-xs text-gray-400 mb-2 truncate">{user?.email}</p>
        <button
          onClick={handleLogout}
          className="w-full text-left text-sm text-gray-300 hover:text-white transition-colors"
        >
          Cerrar sesión
        </button>
      </div>
    </aside>
  )
}
```

- [ ] **Step 6: Crear `src/components/layout/Layout.tsx`**

```tsx
import { Outlet } from 'react-router-dom'
import { Sidebar } from './Sidebar'

export function Layout() {
  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar />
      <main className="flex-1 p-8 overflow-auto">
        <Outlet />
      </main>
    </div>
  )
}
```

- [ ] **Step 7: Commit**

```bash
git add frontend/src/
git commit -m "feat: add Layout, Sidebar, ProtectedRoute with tests"
```

---

## Task 7: App.tsx + main.tsx

**Files:**
- Modify: `frontend/src/App.tsx`
- Modify: `frontend/src/main.tsx`

- [ ] **Step 1: Reemplazar `src/main.tsx`**

```tsx
import React from 'react'
import ReactDOM from 'react-dom/client'
import { Provider } from 'react-redux'
import { BrowserRouter } from 'react-router-dom'
import { store } from './store/store'
import App from './App'
import './index.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <Provider store={store}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </Provider>
  </React.StrictMode>
)
```

- [ ] **Step 2: Reemplazar `src/App.tsx`**

```tsx
import { Routes, Route, Navigate } from 'react-router-dom'
import { ProtectedRoute } from './components/layout/ProtectedRoute'
import { Layout } from './components/layout/Layout'
import { LoginPage } from './pages/auth/LoginPage'
import { RegisterPage } from './pages/auth/RegisterPage'
import { DashboardPage } from './pages/DashboardPage'
import { ProductsPage } from './pages/products/ProductsPage'
import { ProductDetailPage } from './pages/products/ProductDetailPage'
import { CreateProductPage } from './pages/products/CreateProductPage'
import { AlertsPage } from './pages/alerts/AlertsPage'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/products" element={<ProductsPage />} />
          <Route path="/products/new" element={<CreateProductPage />} />
          <Route path="/products/:id" element={<ProductDetailPage />} />
          <Route path="/alerts" element={<AlertsPage />} />
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  )
}
```

> Nota: los imports de páginas darán error de compilación hasta que se creen en los tasks siguientes. Crea archivos placeholder si quieres verificar antes.

- [ ] **Step 3: Commit**

```bash
git add frontend/src/App.tsx frontend/src/main.tsx
git commit -m "feat: configure React Router with protected routes and layout"
```

---

## Task 8: LoginPage (TDD)

**Files:**
- Create: `frontend/src/pages/auth/LoginPage.tsx`
- Create: `frontend/src/pages/auth/LoginPage.test.tsx`

- [ ] **Step 1: Escribir el test en rojo**

Crear `src/pages/auth/LoginPage.test.tsx`:

```tsx
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Routes, Route } from 'react-router-dom'
import { Provider } from 'react-redux'
import { configureStore } from '@reduxjs/toolkit'
import { vi } from 'vitest'
import authReducer from '../../store/authSlice'
import { apiSlice } from '../../api/apiSlice'
import { LoginPage } from './LoginPage'

// Mock del módulo apiSlice para controlar useLoginMutation
vi.mock('../../api/apiSlice', async (importOriginal) => {
  const actual = await importOriginal<typeof import('../../api/apiSlice')>()
  return {
    ...actual,
    useLoginMutation: () => [
      vi.fn().mockReturnValue({
        unwrap: () => Promise.resolve({ token: 'fake-jwt', name: 'Ana', email: 'ana@test.com', role: 'User' }),
      }),
      { isLoading: false, error: null },
    ],
  }
})

function makeStore() {
  return configureStore({
    reducer: { auth: authReducer, [apiSlice.reducerPath]: apiSlice.reducer },
    middleware: (g) => g().concat(apiSlice.middleware),
  })
}

test('submits credentials and saves token in store', async () => {
  const store = makeStore()

  render(
    <Provider store={store}>
      <MemoryRouter initialEntries={['/login']}>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/dashboard" element={<div>Dashboard</div>} />
        </Routes>
      </MemoryRouter>
    </Provider>
  )

  await userEvent.type(screen.getByLabelText(/email/i), 'ana@test.com')
  await userEvent.type(screen.getByLabelText(/contraseña/i), 'password123')
  await userEvent.click(screen.getByRole('button', { name: /iniciar sesión/i }))

  await waitFor(() => {
    expect(store.getState().auth.token).toBe('fake-jwt')
  })
})
```

- [ ] **Step 2: Crear `src/pages/auth/LoginPage.tsx`**

```tsx
import { useState, type FormEvent } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { useDispatch } from 'react-redux'
import { useLoginMutation } from '../../api/apiSlice'
import { setCredentials } from '../../store/authSlice'
import { Spinner } from '../../components/ui/Spinner'

export function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [login, { isLoading, error }] = useLoginMutation()
  const dispatch = useDispatch()
  const navigate = useNavigate()
  const [params] = useSearchParams()

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    try {
      const result = await login({ email, password }).unwrap()
      dispatch(setCredentials({ token: result.token, user: { name: result.name, email: result.email, role: result.role } }))
      navigate(params.get('redirect') ?? '/dashboard', { replace: true })
    } catch {
      // error shown via RTK Query `error` field
    }
  }

  const errorMsg =
    error && 'data' in error
      ? (error.data as { detail?: string })?.detail ?? 'Credenciales incorrectas'
      : undefined

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <div className="bg-white rounded-lg shadow p-8 w-full max-w-md">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Iniciar sesión</h1>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <input
              id="email"
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1">
              Contraseña
            </label>
            <input
              id="password"
              type="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {errorMsg && (
            <p className="text-red-600 text-sm">{errorMsg}</p>
          )}
          <button
            type="submit"
            disabled={isLoading}
            className="w-full bg-blue-600 text-white py-2 rounded font-medium hover:bg-blue-700 disabled:opacity-50 flex justify-center"
          >
            {isLoading ? <Spinner /> : 'Iniciar sesión'}
          </button>
        </form>
        <p className="mt-4 text-sm text-gray-600">
          ¿No tienes cuenta?{' '}
          <Link to="/register" className="text-blue-600 hover:underline">
            Regístrate
          </Link>
        </p>
      </div>
    </div>
  )
}
```

- [ ] **Step 3: Ejecutar todos los tests**

```bash
npm run test
```

Resultado esperado: `✓ 3 tests passed` (2 de ProtectedRoute + 1 de LoginPage).

- [ ] **Step 4: Commit**

```bash
git add frontend/src/pages/auth/LoginPage.tsx frontend/src/pages/auth/LoginPage.test.tsx
git commit -m "feat: add LoginPage with form and JWT dispatch"
```

---

## Task 9: RegisterPage

**Files:**
- Create: `frontend/src/pages/auth/RegisterPage.tsx`

- [ ] **Step 1: Crear `src/pages/auth/RegisterPage.tsx`**

```tsx
import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useDispatch } from 'react-redux'
import { useRegisterMutation } from '../../api/apiSlice'
import { setCredentials } from '../../store/authSlice'
import { Spinner } from '../../components/ui/Spinner'

export function RegisterPage() {
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [register, { isLoading, error }] = useRegisterMutation()
  const dispatch = useDispatch()
  const navigate = useNavigate()

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    try {
      const result = await register({ name, email, password }).unwrap()
      dispatch(setCredentials({ token: result.token, user: { name: result.name, email: result.email, role: result.role } }))
      navigate('/dashboard', { replace: true })
    } catch {
      // error shown via RTK Query `error` field
    }
  }

  const errorMsg =
    error && 'data' in error
      ? (error.data as { detail?: string })?.detail ?? 'Error al registrar'
      : undefined

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <div className="bg-white rounded-lg shadow p-8 w-full max-w-md">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Crear cuenta</h1>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
              Nombre
            </label>
            <input
              id="name"
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <input
              id="email"
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label htmlFor="reg-password" className="block text-sm font-medium text-gray-700 mb-1">
              Contraseña
            </label>
            <input
              id="reg-password"
              type="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {errorMsg && <p className="text-red-600 text-sm">{errorMsg}</p>}
          <button
            type="submit"
            disabled={isLoading}
            className="w-full bg-blue-600 text-white py-2 rounded font-medium hover:bg-blue-700 disabled:opacity-50 flex justify-center"
          >
            {isLoading ? <Spinner /> : 'Registrarse'}
          </button>
        </form>
        <p className="mt-4 text-sm text-gray-600">
          ¿Ya tienes cuenta?{' '}
          <Link to="/login" className="text-blue-600 hover:underline">
            Inicia sesión
          </Link>
        </p>
      </div>
    </div>
  )
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/pages/auth/RegisterPage.tsx
git commit -m "feat: add RegisterPage"
```

---

## Task 10: DashboardPage

**Files:**
- Create: `frontend/src/pages/DashboardPage.tsx`

- [ ] **Step 1: Crear `src/pages/DashboardPage.tsx`**

```tsx
import { useGetProductsQuery, useGetAlertsQuery } from '../api/apiSlice'
import { Spinner } from '../components/ui/Spinner'
import { ErrorMessage } from '../components/ui/ErrorMessage'

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <div className="bg-white rounded-lg shadow p-6">
      <p className="text-sm text-gray-500 mb-1">{label}</p>
      <p className="text-3xl font-bold text-gray-800">{value}</p>
    </div>
  )
}

export function DashboardPage() {
  const { data: products, isLoading: loadingProducts, isError: errorProducts } = useGetProductsQuery()
  const { data: alerts, isLoading: loadingAlerts, isError: errorAlerts } = useGetAlertsQuery()

  if (loadingProducts || loadingAlerts) return <Spinner />
  if (errorProducts || errorAlerts) return <ErrorMessage message="Error al cargar el dashboard." />

  const activeAlerts = alerts?.filter((a) => a.isActive).length ?? 0
  const firedAlerts = alerts?.filter((a) => !a.isActive).length ?? 0

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800 mb-6">Dashboard</h1>
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
        <StatCard label="Productos rastreados" value={products?.length ?? 0} />
        <StatCard label="Alertas activas" value={activeAlerts} />
        <StatCard label="Alertas disparadas" value={firedAlerts} />
      </div>
    </div>
  )
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/pages/DashboardPage.tsx
git commit -m "feat: add DashboardPage with stat cards"
```

---

## Task 11: ProductsPage

**Files:**
- Create: `frontend/src/pages/products/ProductsPage.tsx`

- [ ] **Step 1: Crear `src/pages/products/ProductsPage.tsx`**

```tsx
import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useGetProductsQuery } from '../../api/apiSlice'
import { Spinner } from '../../components/ui/Spinner'
import { ErrorMessage } from '../../components/ui/ErrorMessage'

export function ProductsPage() {
  const { data: products, isLoading, isError } = useGetProductsQuery()
  const [search, setSearch] = useState('')
  const navigate = useNavigate()

  if (isLoading) return <Spinner />
  if (isError) return <ErrorMessage message="Error al cargar los productos." />

  const filtered = products?.filter(
    (p) =>
      p.name.toLowerCase().includes(search.toLowerCase()) ||
      p.category.toLowerCase().includes(search.toLowerCase())
  ) ?? []

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Productos</h1>
        <button
          onClick={() => navigate('/products/new')}
          className="bg-blue-600 text-white px-4 py-2 rounded text-sm font-medium hover:bg-blue-700"
        >
          + Nuevo producto
        </button>
      </div>
      <input
        type="text"
        placeholder="Buscar por nombre o categoría..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="w-full border border-gray-300 rounded px-3 py-2 text-sm mb-4 focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
      {filtered.length === 0 ? (
        <p className="text-gray-500 text-sm">No se encontraron productos.</p>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-gray-600 uppercase text-xs">
              <tr>
                <th className="px-4 py-3 text-left">Nombre</th>
                <th className="px-4 py-3 text-left">Categoría</th>
                <th className="px-4 py-3 text-left">Descripción</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filtered.map((product) => (
                <tr
                  key={product.id}
                  onClick={() => navigate(`/products/${product.id}`)}
                  className="hover:bg-gray-50 cursor-pointer"
                >
                  <td className="px-4 py-3 font-medium text-blue-600">{product.name}</td>
                  <td className="px-4 py-3 text-gray-600">{product.category}</td>
                  <td className="px-4 py-3 text-gray-500 truncate max-w-xs">{product.description}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/pages/products/ProductsPage.tsx
git commit -m "feat: add ProductsPage with search filter"
```

---

## Task 12: ProductDetailPage

**Files:**
- Create: `frontend/src/pages/products/ProductDetailPage.tsx`

- [ ] **Step 1: Crear `src/pages/products/ProductDetailPage.tsx`**

```tsx
import { useState, type FormEvent } from 'react'
import { useParams } from 'react-router-dom'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts'
import { useGetProductByIdQuery, useGetPriceHistoryQuery, useCreateAlertMutation } from '../../api/apiSlice'
import { Spinner } from '../../components/ui/Spinner'
import { ErrorMessage } from '../../components/ui/ErrorMessage'

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6']

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: product, isLoading: loadingProduct, isError: errorProduct } = useGetProductByIdQuery(id!)
  const { data: prices, isLoading: loadingPrices, isError: errorPrices } = useGetPriceHistoryQuery(id!)
  const [createAlert, { isLoading: creatingAlert, error: alertError }] = useCreateAlertMutation()
  const [targetPrice, setTargetPrice] = useState('')
  const [alertCreated, setAlertCreated] = useState(false)

  if (loadingProduct || loadingPrices) return <Spinner />
  if (errorProduct || errorPrices) return <ErrorMessage message="Error al cargar el producto." />

  // Agrupar precios por tienda para Recharts
  const stores = [...new Set(prices?.map((p) => p.storeName) ?? [])]
  const dateMap = new Map<string, Record<string, number>>()
  prices?.forEach((p) => {
    const date = new Date(p.dateCollected).toLocaleDateString('es-ES')
    if (!dateMap.has(date)) dateMap.set(date, { date } as Record<string, number>)
    dateMap.get(date)![p.storeName] = p.price
  })
  const chartData = Array.from(dateMap.values())

  async function handleCreateAlert(e: FormEvent) {
    e.preventDefault()
    await createAlert({ productId: id!, targetPrice: parseFloat(targetPrice) }).unwrap()
    setAlertCreated(true)
    setTargetPrice('')
  }

  const alertErr =
    alertError && 'data' in alertError
      ? (alertError.data as { detail?: string })?.detail ?? 'Error al crear la alerta'
      : undefined

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-gray-800">{product?.name}</h1>
        <p className="text-sm text-gray-500 mt-1">
          <span className="bg-gray-100 px-2 py-0.5 rounded">{product?.category}</span>
        </p>
        <p className="text-gray-600 mt-2">{product?.description}</p>
      </div>

      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-700 mb-4">Histórico de precios</h2>
        {chartData.length === 0 ? (
          <p className="text-gray-400 text-sm">No hay datos de precios aún.</p>
        ) : (
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="date" tick={{ fontSize: 11 }} />
              <YAxis tick={{ fontSize: 11 }} unit="€" />
              <Tooltip formatter={(value) => `${value}€`} />
              <Legend />
              {stores.map((store, i) => (
                <Line
                  key={store}
                  type="monotone"
                  dataKey={store}
                  stroke={COLORS[i % COLORS.length]}
                  dot={false}
                  strokeWidth={2}
                />
              ))}
            </LineChart>
          </ResponsiveContainer>
        )}
      </div>

      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-700 mb-4">Crear alerta de precio</h2>
        {alertCreated && (
          <p className="text-green-600 text-sm mb-3">¡Alerta creada correctamente!</p>
        )}
        <form onSubmit={handleCreateAlert} className="flex gap-3 items-end">
          <div className="flex-1">
            <label htmlFor="targetPrice" className="block text-sm font-medium text-gray-700 mb-1">
              Precio objetivo (€)
            </label>
            <input
              id="targetPrice"
              type="number"
              step="0.01"
              min="0"
              required
              value={targetPrice}
              onChange={(e) => setTargetPrice(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <button
            type="submit"
            disabled={creatingAlert}
            className="bg-blue-600 text-white px-4 py-2 rounded text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
          >
            {creatingAlert ? '...' : 'Crear alerta'}
          </button>
        </form>
        {alertErr && <p className="text-red-600 text-sm mt-2">{alertErr}</p>}
      </div>
    </div>
  )
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/pages/products/ProductDetailPage.tsx
git commit -m "feat: add ProductDetailPage with Recharts price history"
```

---

## Task 13: CreateProductPage

**Files:**
- Create: `frontend/src/pages/products/CreateProductPage.tsx`

- [ ] **Step 1: Crear `src/pages/products/CreateProductPage.tsx`**

```tsx
import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCreateProductMutation } from '../../api/apiSlice'
import { Spinner } from '../../components/ui/Spinner'

export function CreateProductPage() {
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [category, setCategory] = useState('')
  const [createProduct, { isLoading, error }] = useCreateProductMutation()
  const navigate = useNavigate()

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    await createProduct({ name, description, category }).unwrap()
    navigate('/products')
  }

  const errorMsg =
    error && 'data' in error
      ? (error.data as { detail?: string })?.detail ?? 'Error al crear el producto'
      : undefined

  return (
    <div className="max-w-lg">
      <h1 className="text-2xl font-bold text-gray-800 mb-6">Nuevo producto</h1>
      <div className="bg-white rounded-lg shadow p-6">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="prod-name" className="block text-sm font-medium text-gray-700 mb-1">
              Nombre
            </label>
            <input
              id="prod-name"
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label htmlFor="prod-category" className="block text-sm font-medium text-gray-700 mb-1">
              Categoría
            </label>
            <input
              id="prod-category"
              type="text"
              required
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label htmlFor="prod-desc" className="block text-sm font-medium text-gray-700 mb-1">
              Descripción
            </label>
            <textarea
              id="prod-desc"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {errorMsg && <p className="text-red-600 text-sm">{errorMsg}</p>}
          <div className="flex gap-3">
            <button
              type="button"
              onClick={() => navigate('/products')}
              className="flex-1 border border-gray-300 text-gray-700 py-2 rounded font-medium hover:bg-gray-50"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="flex-1 bg-blue-600 text-white py-2 rounded font-medium hover:bg-blue-700 disabled:opacity-50 flex justify-center"
            >
              {isLoading ? <Spinner /> : 'Guardar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/pages/products/CreateProductPage.tsx
git commit -m "feat: add CreateProductPage"
```

---

## Task 14: AlertsPage

**Files:**
- Create: `frontend/src/pages/alerts/AlertsPage.tsx`

- [ ] **Step 1: Crear `src/pages/alerts/AlertsPage.tsx`**

```tsx
import { useState, type FormEvent } from 'react'
import { useGetAlertsQuery, useCreateAlertMutation, useDeleteAlertMutation, useGetProductsQuery } from '../../api/apiSlice'
import { Spinner } from '../../components/ui/Spinner'
import { ErrorMessage } from '../../components/ui/ErrorMessage'

export function AlertsPage() {
  const { data: alerts, isLoading, isError } = useGetAlertsQuery()
  const { data: products } = useGetProductsQuery()
  const [createAlert, { isLoading: creating, error: createError }] = useCreateAlertMutation()
  const [deleteAlert] = useDeleteAlertMutation()
  const [productId, setProductId] = useState('')
  const [targetPrice, setTargetPrice] = useState('')
  const [deletingId, setDeletingId] = useState<string | null>(null)

  if (isLoading) return <Spinner />
  if (isError) return <ErrorMessage message="Error al cargar las alertas." />

  async function handleCreate(e: FormEvent) {
    e.preventDefault()
    await createAlert({ productId, targetPrice: parseFloat(targetPrice) }).unwrap()
    setProductId('')
    setTargetPrice('')
  }

  async function handleDelete(id: string) {
    setDeletingId(id)
    await deleteAlert(id)
    setDeletingId(null)
  }

  const createErr =
    createError && 'data' in createError
      ? (createError.data as { detail?: string })?.detail ?? 'Error al crear la alerta'
      : undefined

  return (
    <div className="space-y-8">
      <h1 className="text-2xl font-bold text-gray-800">Mis alertas</h1>

      {/* Lista de alertas */}
      {alerts?.length === 0 ? (
        <p className="text-gray-500 text-sm">No tienes alertas configuradas.</p>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-gray-600 uppercase text-xs">
              <tr>
                <th className="px-4 py-3 text-left">Producto</th>
                <th className="px-4 py-3 text-left">Precio objetivo</th>
                <th className="px-4 py-3 text-left">Estado</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {alerts?.map((alert) => (
                <tr key={alert.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium text-gray-800">{alert.productName}</td>
                  <td className="px-4 py-3 text-gray-600">{alert.targetPrice.toFixed(2)} €</td>
                  <td className="px-4 py-3">
                    <span
                      className={`inline-block px-2 py-0.5 rounded text-xs font-medium ${
                        alert.isActive
                          ? 'bg-green-100 text-green-700'
                          : 'bg-gray-100 text-gray-500'
                      }`}
                    >
                      {alert.isActive ? 'Activa' : 'Disparada'}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-right">
                    <button
                      onClick={() => handleDelete(alert.id)}
                      disabled={deletingId === alert.id}
                      className="text-red-500 hover:text-red-700 text-xs font-medium disabled:opacity-50"
                    >
                      {deletingId === alert.id ? '...' : 'Eliminar'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Formulario de nueva alerta */}
      <div className="bg-white rounded-lg shadow p-6 max-w-lg">
        <h2 className="text-lg font-semibold text-gray-700 mb-4">Nueva alerta</h2>
        <form onSubmit={handleCreate} className="space-y-4">
          <div>
            <label htmlFor="alert-product" className="block text-sm font-medium text-gray-700 mb-1">
              Producto
            </label>
            <select
              id="alert-product"
              required
              value={productId}
              onChange={(e) => setProductId(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Selecciona un producto...</option>
              {products?.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label htmlFor="alert-price" className="block text-sm font-medium text-gray-700 mb-1">
              Precio objetivo (€)
            </label>
            <input
              id="alert-price"
              type="number"
              step="0.01"
              min="0"
              required
              value={targetPrice}
              onChange={(e) => setTargetPrice(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {createErr && <p className="text-red-600 text-sm">{createErr}</p>}
          <button
            type="submit"
            disabled={creating}
            className="w-full bg-blue-600 text-white py-2 rounded font-medium hover:bg-blue-700 disabled:opacity-50"
          >
            {creating ? 'Creando...' : 'Crear alerta'}
          </button>
        </form>
      </div>
    </div>
  )
}
```

- [ ] **Step 2: Commit**

```bash
git add frontend/src/pages/alerts/AlertsPage.tsx
git commit -m "feat: add AlertsPage with create and delete"
```

---

## Task 15: Verificación final

- [ ] **Step 1: Ejecutar todos los tests**

```bash
cd frontend
npm run test
```

Resultado esperado:
```
✓ ProtectedRoute > redirects to /login when not authenticated
✓ ProtectedRoute > renders children when authenticated
✓ LoginPage > submits credentials and redirects on success  (o skip si axios mock no aplica)
```

- [ ] **Step 2: Verificar build de producción**

```bash
npm run build
```

Resultado esperado: `dist/` generado sin errores TypeScript.

- [ ] **Step 3: Levantar backend y probar manualmente**

```bash
# Terminal 1 — PostgreSQL
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=pricetracker postgres:16

# Terminal 2 — Backend
cd ../backend
dotnet ef database update --project PriceTrackerCloud.Infrastructure --startup-project PriceTrackerCloud.API
dotnet run --project PriceTrackerCloud.API

# Terminal 3 — Frontend
cd ../frontend
cp .env.example .env.local
npm run dev
```

Flujo a verificar:
1. Abrir `http://localhost:5173` → redirige a `/login`
2. Registrarse → redirige a `/dashboard` con 3 tarjetas
3. Ir a `/products` → ver los 3 productos seed (PS5, iPhone 15 Pro, MacBook Pro)
4. Clicar un producto → ver gráfica de precios con 2 líneas (Amazon, PcComponentes)
5. Crear una alerta → aparece en `/alerts`
6. Eliminar la alerta → desaparece de la lista
7. Logout → redirige a `/login`

- [ ] **Step 4: Commit final**

```bash
cd ..
git add frontend/
git commit -m "feat: Phase 5 complete — React frontend connected to API"
```

---

## Checklist de criterios de aceptación

- [ ] `npm run dev` arranca sin errores en `http://localhost:5173`
- [ ] Login/Register funciona contra la API real
- [ ] Rutas protegidas redirigen a `/login` sin token y vuelven al origen tras autenticarse
- [ ] Dashboard muestra las 3 tarjetas con datos reales
- [ ] Gráfica de precios renderiza con los datos seed
- [ ] Crear producto y alerta funcionan y actualizan las listas
- [ ] Eliminar alerta funciona
- [ ] `npm run build` genera `dist/` sin errores TypeScript
- [ ] `npm run test` → tests de ProtectedRoute en verde
