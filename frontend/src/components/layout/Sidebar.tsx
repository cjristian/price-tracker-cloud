import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'
import { logoutAndReset } from '../../store/store'

const links = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/products', label: 'Productos' },
  { to: '/alerts', label: 'Alertas' },
]

export function Sidebar() {
  const navigate = useNavigate()
  const { user } = useAuth()

  function handleLogout() {
    logoutAndReset()
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
