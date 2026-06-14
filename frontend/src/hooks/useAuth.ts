import { useSelector } from 'react-redux'
import type { RootState } from '../store/store'

export function useAuth() {
  const token = useSelector((state: RootState) => state.auth.token)
  const user = useSelector((state: RootState) => state.auth.user)
  return { token, user, isAuthenticated: token !== null }
}
