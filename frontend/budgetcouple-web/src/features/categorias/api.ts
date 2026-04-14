import { api } from '@/lib/api'
import type { Categoria, CategoriaFormData } from './types'

export const categoriasApi = {
  list: async (): Promise<Categoria[]> => {
    const { data } = await api.get<Categoria[]>('/categorias')
    return data
  },

  getById: async (id: string): Promise<Categoria> => {
    const { data } = await api.get<Categoria>(`/categorias/${id}`)
    return data
  },

  create: async (input: CategoriaFormData): Promise<Categoria> => {
    const { data } = await api.post<Categoria>('/categorias', input)
    return data
  },

  update: async (id: string, input: CategoriaFormData): Promise<Categoria> => {
    const { data } = await api.put<Categoria>(`/categorias/${id}`, input)
    return data
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/categorias/${id}`)
  },
}
