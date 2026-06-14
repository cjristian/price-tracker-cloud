import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Routes, Route } from 'react-router-dom'
import { Provider } from 'react-redux'
import { configureStore } from '@reduxjs/toolkit'
import { vi } from 'vitest'
import authReducer from '../../store/authSlice'
import { apiSlice } from '../../api/apiSlice'
import { LoginPage } from './LoginPage'

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
