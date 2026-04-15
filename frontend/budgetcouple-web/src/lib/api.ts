import axios from 'axios'

/**
 * Normalize the API base URL so it ALWAYS ends with /api/v1.
 * This protects against misconfigured VITE_API_URL (e.g. missing the path suffix)
 * and avoids runtime 404s when calling endpoints like /auth/login.
 */
export function resolveBaseUrl(url?: string): string {
  const raw = url ?? import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api/v1'
  const trimmed = raw.replace(/\/+$/, '') // strip trailing slashes
  if (/\/api\/v\d+$/.test(trimmed)) return trimmed
  return `${trimmed}/api/v1`
}

export const API_BASE_URL = resolveBaseUrl()

export const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('bc_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
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
