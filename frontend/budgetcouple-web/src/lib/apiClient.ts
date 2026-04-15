import axios from 'axios'
import { API_BASE_URL } from './api'

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
})

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('bc_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Only redirect if not already on auth endpoints
      if (!error.config.url?.includes('/auth/')) {
        localStorage.removeItem('bc_token')
        localStorage.removeItem('bc_expires')
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)
