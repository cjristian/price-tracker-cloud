import { createApi } from '@reduxjs/toolkit/query/react'
import { axiosBaseQuery } from './axiosBase'
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  Product,
  CreateProductRequest,
  ProductPrice,
  Alert,
  CreateAlertRequest,
} from '../types'

export const apiSlice = createApi({
  reducerPath: 'api',
  baseQuery: axiosBaseQuery(),
  tagTypes: ['Products', 'Alerts', 'Prices'],
  endpoints: (builder) => ({
    login: builder.mutation<AuthResponse, LoginRequest>({
      query: (body) => ({ url: '/auth/login', method: 'POST', data: body }),
    }),
    register: builder.mutation<AuthResponse, RegisterRequest>({
      query: (body) => ({ url: '/auth/register', method: 'POST', data: body }),
    }),
    getProducts: builder.query<Product[], void>({
      query: () => ({ url: '/products' }),
      providesTags: ['Products'],
    }),
    getProductById: builder.query<Product, string>({
      query: (id) => ({ url: `/products/${id}` }),
      providesTags: ['Products'],
    }),
    createProduct: builder.mutation<Product, CreateProductRequest>({
      query: (body) => ({ url: '/products', method: 'POST', data: body }),
      invalidatesTags: ['Products'],
    }),
    getPriceHistory: builder.query<ProductPrice[], string>({
      query: (productId) => ({ url: `/prices/history/${productId}` }),
      providesTags: ['Prices'],
    }),
    getAlerts: builder.query<Alert[], void>({
      query: () => ({ url: '/alerts' }),
      providesTags: ['Alerts'],
    }),
    createAlert: builder.mutation<Alert, CreateAlertRequest>({
      query: (body) => ({ url: '/alerts', method: 'POST', data: body }),
      invalidatesTags: ['Alerts'],
    }),
    deleteAlert: builder.mutation<void, string>({
      query: (id) => ({ url: `/alerts/${id}`, method: 'DELETE' }),
      invalidatesTags: ['Alerts'],
    }),
  }),
})

export const {
  useLoginMutation,
  useRegisterMutation,
  useGetProductsQuery,
  useGetProductByIdQuery,
  useCreateProductMutation,
  useGetPriceHistoryQuery,
  useGetAlertsQuery,
  useCreateAlertMutation,
  useDeleteAlertMutation,
} = apiSlice
