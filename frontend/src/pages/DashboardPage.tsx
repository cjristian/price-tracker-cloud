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
