import { api } from '@/lib/api'
import type { Conta, ContaFormData } from './types'

export const contasApi = {
  list: async (): Promise<Conta[]> => {
    const { data } = await api.get<Conta[]>('/contas')
    return data
  },

  getById: async (id: string): Promise<Conta> => {
    const { data } = await api.get<Conta>(`/contas/${id}`)
    return data
  },

  create: async (input: ContaFormData): Promise<Conta> => {
    const { data } = await api.post<Conta>('/contas', input)
    return data
  },

  update: async (id: string, input: ContaFormData): Promise<Conta> => {
    const { data } = await api.put<Conta>(`/contas/${id}`, input)
    return data
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/contas/${id}`)
  },
}
