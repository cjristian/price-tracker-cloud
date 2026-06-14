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
