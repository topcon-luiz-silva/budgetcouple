import { api } from '@/lib/api'
import type { Cartao, CartaoFormData } from './types'

export const cartoesApi = {
  list: async (): Promise<Cartao[]> => {
    const { data } = await api.get<Cartao[]>('/cartoes')
    return data
  },

  getById: async (id: string): Promise<Cartao> => {
    const { data } = await api.get<Cartao>(`/cartoes/${id}`)
    return data
  },

  create: async (input: CartaoFormData): Promise<Cartao> => {
    const { data } = await api.post<Cartao>('/cartoes', input)
    return data
  },

  update: async (id: string, input: CartaoFormData): Promise<Cartao> => {
    const { data } = await api.put<Cartao>(`/cartoes/${id}`, input)
    return data
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/cartoes/${id}`)
  },
}
