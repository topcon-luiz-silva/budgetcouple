import { api } from '@/lib/api'
import type { RegraClassificacao, RegraFormData } from './types'

export const regrasApi = {
  list: async (apenasAtivas?: boolean): Promise<RegraClassificacao[]> => {
    const params = new URLSearchParams()
    if (apenasAtivas !== undefined) {
      params.append('apenasAtivas', String(apenasAtivas))
    }

    const url = `/regras-classificacao${params.toString() ? `?${params}` : ''}`
    const { data } = await api.get<RegraClassificacao[]>(url)
    return data
  },

  getById: async (id: string): Promise<RegraClassificacao> => {
    const { data } = await api.get<RegraClassificacao>(`/regras-classificacao/${id}`)
    return data
  },

  create: async (input: RegraFormData): Promise<{ id: string }> => {
    const { data } = await api.post<{ id: string }>('/regras-classificacao', input)
    return data
  },

  update: async (id: string, input: RegraFormData): Promise<void> => {
    await api.put(`/regras-classificacao/${id}`, input)
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/regras-classificacao/${id}`)
  },
}
