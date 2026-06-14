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
