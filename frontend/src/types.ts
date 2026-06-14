export interface AuthResponse {
  token: string
  name: string
  email: string
  role: string
}

export interface Product {
  id: string
  name: string
  description: string
  category: string
}

export interface ProductPrice {
  id: string
  productId: string
  storeName: string
  price: number
  dateCollected: string
}

export interface Alert {
  id: string
  productId: string
  productName: string
  targetPrice: number
  isActive: boolean
  createdAt: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  name: string
  email: string
  password: string
}

export interface CreateProductRequest {
  name: string
  description: string
  category: string
}

export interface CreateAlertRequest {
  productId: string
  targetPrice: number
}
