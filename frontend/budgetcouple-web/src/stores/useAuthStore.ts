import { create } from 'zustand'

export interface AuthState {
  token: string | null
  expiresAt: string | null
  setAuth: (token: string, expiresAt: string) => void
  clear: () => void
  isAuthenticated: () => boolean
}

export const useAuthStore = create<AuthState>((set, get) => ({
  token: localStorage.getItem('bc_token'),
  expiresAt: localStorage.getItem('bc_expires'),

  setAuth: (token: string, expiresAt: string) => {
    localStorage.setItem('bc_token', token)
    localStorage.setItem('bc_expires', expiresAt)
    set({ token, expiresAt })
  },

  clear: () => {
    localStorage.removeItem('bc_token')
    localStorage.removeItem('bc_expires')
    set({ token: null, expiresAt: null })
  },

  isAuthenticated: () => {
    const { token, expiresAt } = get()
    if (!token || !expiresAt) return false
    const expires = new Date(expiresAt)
    return expires > new Date()
  },
}))
