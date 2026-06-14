import { useState, type FormEvent } from 'react'
import { useParams } from 'react-router-dom'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts'
import { useGetProductByIdQuery, useGetPriceHistoryQuery, useCreateAlertMutation } from '../../api/apiSlice'
import { Spinner } from '../../components/ui/Spinner'
import { ErrorMessage } from '../../components/ui/ErrorMessage'

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6']

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: product, isLoading: loadingProduct, isError: errorProduct } = useGetProductByIdQuery(id!)
  const { data: prices, isLoading: loadingPrices, isError: errorPrices } = useGetPriceHistoryQuery(id!)
  const [createAlert, { isLoading: creatingAlert, error: alertError }] = useCreateAlertMutation()
  const [targetPrice, setTargetPrice] = useState('')
  const [alertCreated, setAlertCreated] = useState(false)

  if (loadingProduct || loadingPrices) return <Spinner />
  if (errorProduct || errorPrices) return <ErrorMessage message="Error al cargar el producto." />

  const stores = [...new Set(prices?.map((p) => p.storeName) ?? [])]
  const dateMap = new Map<string, Record<string, unknown>>()
  prices?.forEach((p) => {
    const date = new Date(p.dateCollected).toLocaleDateString('es-ES')
    if (!dateMap.has(date)) dateMap.set(date, { date })
    dateMap.get(date)![p.storeName] = p.price
  })
  const chartData = Array.from(dateMap.values())

  async function handleCreateAlert(e: FormEvent) {
    e.preventDefault()
    await createAlert({ productId: id!, targetPrice: parseFloat(targetPrice) }).unwrap()
    setAlertCreated(true)
    setTargetPrice('')
  }

  const alertErr =
    alertError && 'data' in alertError
      ? (alertError.data as { detail?: string })?.detail ?? 'Error al crear la alerta'
      : undefined

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-gray-800">{product?.name}</h1>
        <p className="text-sm text-gray-500 mt-1">
          <span className="bg-gray-100 px-2 py-0.5 rounded">{product?.category}</span>
        </p>
        <p className="text-gray-600 mt-2">{product?.description}</p>
      </div>

      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-700 mb-4">Histórico de precios</h2>
        {chartData.length === 0 ? (
          <p className="text-gray-400 text-sm">No hay datos de precios aún.</p>
        ) : (
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="date" tick={{ fontSize: 11 }} />
              <YAxis tick={{ fontSize: 11 }} unit="€" />
              <Tooltip formatter={(value) => `${value}€`} />
              <Legend />
              {stores.map((store, i) => (
                <Line
                  key={store}
                  type="monotone"
                  dataKey={store}
                  stroke={COLORS[i % COLORS.length]}
                  dot={false}
                  strokeWidth={2}
                />
              ))}
            </LineChart>
          </ResponsiveContainer>
        )}
      </div>

      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-700 mb-4">Crear alerta de precio</h2>
        {alertCreated && (
          <p className="text-green-600 text-sm mb-3">¡Alerta creada correctamente!</p>
        )}
        <form onSubmit={handleCreateAlert} className="flex gap-3 items-end">
          <div className="flex-1">
            <label htmlFor="targetPrice" className="block text-sm font-medium text-gray-700 mb-1">
              Precio objetivo (€)
            </label>
            <input
              id="targetPrice"
              type="number"
              step="0.01"
              min="0"
              required
              value={targetPrice}
              onChange={(e) => setTargetPrice(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <button
            type="submit"
            disabled={creatingAlert}
            className="bg-blue-600 text-white px-4 py-2 rounded text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
          >
            {creatingAlert ? '...' : 'Crear alerta'}
          </button>
        </form>
        {alertErr && <p className="text-red-600 text-sm mt-2">{alertErr}</p>}
      </div>
    </div>
  )
}
