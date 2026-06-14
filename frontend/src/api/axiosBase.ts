import axios from 'axios'
import type { BaseQueryFn } from '@reduxjs/toolkit/query'
import type { AxiosRequestConfig, AxiosError } from 'axios'
import { logout } from '../store/authSlice'
import type { RootState } from '../store/store'

export const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5295',
})

type AxiosArgs = {
  url: string
  method?: AxiosRequestConfig['method']
  data?: unknown
  params?: unknown
}

export const axiosBaseQuery =
  (): BaseQueryFn<AxiosArgs, unknown, { status?: number; data: unknown }> =>
  async ({ url, method = 'GET', data, params }, api) => {
    const token = (api.getState() as RootState).auth.token
    try {
      const result = await axiosInstance({
        url,
        method,
        data,
        params,
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      })
      return { data: result.data }
    } catch (err) {
      const axiosErr = err as AxiosError
      if (axiosErr.response?.status === 401) {
        api.dispatch(logout())
      }
      return {
        error: {
          status: axiosErr.response?.status,
          data: axiosErr.response?.data ?? axiosErr.message,
        },
      }
    }
  }
