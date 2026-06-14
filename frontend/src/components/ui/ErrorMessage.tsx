interface Props {
  message?: string
}

export function ErrorMessage({ message = 'Ha ocurrido un error.' }: Props) {
  return (
    <div className="bg-red-50 border border-red-300 text-red-700 rounded p-4 text-sm">
      {message}
    </div>
  )
}
