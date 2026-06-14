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
