import { configureStore } from '@reduxjs/toolkit'
import authReducer, { logout } from './authSlice'
import { apiSlice } from '../api/apiSlice'

export const store = configureStore({
  reducer: {
    auth: authReducer,
    [apiSlice.reducerPath]: apiSlice.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(apiSlice.middleware),
})

// Clears both auth state and RTK Query cache on logout
export function logoutAndReset() {
  store.dispatch(logout())
  store.dispatch(apiSlice.util.resetApiState())
  localStorage.removeItem('auth')
}

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
