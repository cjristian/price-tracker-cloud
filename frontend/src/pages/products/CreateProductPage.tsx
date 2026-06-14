import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCreateProductMutation } from '../../api/apiSlice'
import { Spinner } from '../../components/ui/Spinner'

export function CreateProductPage() {
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [category, setCategory] = useState('')
  const [createProduct, { isLoading, error }] = useCreateProductMutation()
  const navigate = useNavigate()

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    await createProduct({ name, description, category }).unwrap()
    navigate('/products')
  }

  const errorMsg =
    error && 'data' in error
      ? (error.data as { detail?: string })?.detail ?? 'Error al crear el producto'
      : undefined

  return (
    <div className="max-w-lg">
      <h1 className="text-2xl font-bold text-gray-800 mb-6">Nuevo producto</h1>
      <div className="bg-white rounded-lg shadow p-6">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="prod-name" className="block text-sm font-medium text-gray-700 mb-1">
              Nombre
            </label>
            <input
              id="prod-name"
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label htmlFor="prod-category" className="block text-sm font-medium text-gray-700 mb-1">
              Categoría
            </label>
            <input
              id="prod-category"
              type="text"
              required
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label htmlFor="prod-desc" className="block text-sm font-medium text-gray-700 mb-1">
              Descripción
            </label>
            <textarea
              id="prod-desc"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
              className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {errorMsg && <p className="text-red-600 text-sm">{errorMsg}</p>}
          <div className="flex gap-3">
            <button
              type="button"
              onClick={() => navigate('/products')}
              className="flex-1 border border-gray-300 text-gray-700 py-2 rounded font-medium hover:bg-gray-50"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="flex-1 bg-blue-600 text-white py-2 rounded font-medium hover:bg-blue-700 disabled:opacity-50 flex justify-center"
            >
              {isLoading ? <Spinner /> : 'Guardar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
