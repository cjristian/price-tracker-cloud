import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useGetProductsQuery } from '../../api/apiSlice'
import { Spinner } from '../../components/ui/Spinner'
import { ErrorMessage } from '../../components/ui/ErrorMessage'

export function ProductsPage() {
  const { data: products, isLoading, isError } = useGetProductsQuery()
  const [search, setSearch] = useState('')
  const navigate = useNavigate()

  if (isLoading) return <Spinner />
  if (isError) return <ErrorMessage message="Error al cargar los productos." />

  const filtered = products?.filter(
    (p) =>
      p.name.toLowerCase().includes(search.toLowerCase()) ||
      p.category.toLowerCase().includes(search.toLowerCase())
  ) ?? []

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Productos</h1>
        <button
          onClick={() => navigate('/products/new')}
          className="bg-blue-600 text-white px-4 py-2 rounded text-sm font-medium hover:bg-blue-700"
        >
          + Nuevo producto
        </button>
      </div>
      <input
        type="text"
        placeholder="Buscar por nombre o categoría..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="w-full border border-gray-300 rounded px-3 py-2 text-sm mb-4 focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
      {filtered.length === 0 ? (
        <p className="text-gray-500 text-sm">No se encontraron productos.</p>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-gray-600 uppercase text-xs">
              <tr>
                <th className="px-4 py-3 text-left">Nombre</th>
                <th className="px-4 py-3 text-left">Categoría</th>
                <th className="px-4 py-3 text-left">Descripción</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filtered.map((product) => (
                <tr
                  key={product.id}
                  onClick={() => navigate(`/products/${product.id}`)}
                  className="hover:bg-gray-50 cursor-pointer"
                >
                  <td className="px-4 py-3 font-medium text-blue-600">{product.name}</td>
                  <td className="px-4 py-3 text-gray-600">{product.category}</td>
                  <td className="px-4 py-3 text-gray-500 truncate max-w-xs">{product.description}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
