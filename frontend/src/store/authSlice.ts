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

function loadAuthState(): AuthState {
  try {
    const raw = localStorage.getItem('auth')
    if (!raw) return { token: null, user: null }
    const parsed = JSON.parse(raw)
    if (typeof parsed?.token === 'string' && parsed.user && typeof parsed.user.role === 'string') {
      return parsed
    }
    return { token: null, user: null }
  } catch {
    localStorage.removeItem('auth')
    return { token: null, user: null }
  }
}

const authSlice = createSlice({
  name: 'auth',
  initialState: loadAuthState(),
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
