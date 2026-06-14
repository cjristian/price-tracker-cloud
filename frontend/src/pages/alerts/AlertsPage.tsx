import { useState, type FormEvent } from 'react'
import { useGetAlertsQuery, useCreateAlertMutation, useDeleteAlertMutation, useGetProductsQuery } from '../../api/apiSlice'
import { Spinner } from '../../components/ui/Spinner'
import { ErrorMessage } from '../../components/ui/ErrorMessage'

export function AlertsPage() {
  const { data: alerts, isLoading, isError } = useGetAlertsQuery()
  const { data: products } = useGetProductsQuery()
  const [createAlert, { isLoading: creating, error: createError }] = useCreateAlertMutation()
  const [deleteAlert] = useDeleteAlertMutation()
  const [productId, setProductId] = useState('')
  const [targetPrice, setTargetPrice] = useState('')
  const [deletingId, setDeletingId] = useState<string | null>(null)

  if (isLoading) return <Spinner />
  if (isError) return <ErrorMessage message="Error al cargar las alertas." />

  async function handleCreate(e: FormEvent) {
    e.preventDefault()
    await createAlert({ productId, targetPrice: parseFloat(targetPrice) }).unwrap()
    setProductId('')
    setTargetPrice('')
  }

  async function handleDelete(id: string) {
    setDeletingId(id)
    await deleteAlert(id)
    setDeletingId(null)
  }

  const createErr =
    createError && 'data' in createError
      ? (createError.data as { detail?: string })?.detail ?? 'Error al crear la alerta'
      : undefined

  return (
    <div className="space-y-8">
      <h1 className="text-2xl font-bold text-gray-800">Mis alertas</h1>

      {alerts?.length === 0 ? (
        <p className="text-gray-500 text-sm">No tienes alertas configuradas.</p>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-gray-600 uppercase text-xs">
              <tr>
                <th className="px-4 py-3 text-left">Producto</th>
                <th className="px-4 py-3 text-left">Precio objetivo</th>
                <th className="px-4 py-3 text-left">Estado</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {alerts?.map((alert) => (
                <tr key={alert.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium text-gray-800">{alert.productName}</td>
                  <td className="px-4 py-3 text-gray-600">{alert.targetPrice.toFixed(2)} €</td>
                  <td className="px-4 py-3">
                    <span
                      className={`inline-block px-2 py-0.5 rounded text-xs font-medium ${
                        alert.isActive
                          ? 'bg-green-100 text-green-700'
                          : 'bg-gray-100 text-gray-500'
                      }`}
                    >
                      {alert.isActive ? 'Activa' : 'Disparada'}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-right">
                    <button
                      onClick={() => handleDelete(alert.id)}
                      disabled={deletingId === alert.id}
                      className="text-red-500 hover:text-red-700 text-xs font-medium disabled:opacity-50"
                    >
                      {deletingId === alert.id ? '...' : 'Eliminar'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="bg-white rounded-lg shadow p-6 max-w-lg">
        <h2 className="text-lg font-semibold text-gray-700 mb-4">Nueva alerta</h2>
        <form onSubmit={handleCreate} className="space-y-4">
          <div>
            <label htmlFor="alert-product" className="block text-sm font-medium text-gray-700 mb-1">
              Producto
            </label>
            <select
              id="alert-product"
              required
              value={productId}
              onChange={(e) => setProductId(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Selecciona un producto...</option>
              {products?.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label htmlFor="alert-price" className="block text-sm font-medium text-gray-700 mb-1">
              Precio objetivo (€)
            </label>
            <input
              id="alert-price"
              type="number"
              step="0.01"
              min="0"
              required
              value={targetPrice}
              onChange={(e) => setTargetPrice(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {createErr && <p className="text-red-600 text-sm">{createErr}</p>}
          <button
            type="submit"
            disabled={creating}
            className="w-full bg-blue-600 text-white py-2 rounded font-medium hover:bg-blue-700 disabled:opacity-50"
          >
            {creating ? 'Creando...' : 'Crear alerta'}
          </button>
        </form>
      </div>
    </div>
  )
}
