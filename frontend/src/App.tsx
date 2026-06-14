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
