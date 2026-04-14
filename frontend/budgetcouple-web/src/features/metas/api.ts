import { api } from '@/lib/api'
import type {
  Meta,
  MetaProgresso,
  AlertaOrcamentoDashboard,
  CreateMetaInput,
  UpdateMetaInput,
} from './types'

export const metasApi = {
  listMetas: async (): Promise<Meta[]> => {
    const response = await api.get<Meta[]>('/metas')
    return response.data
  },

  getMeta: async (id: string): Promise<Meta> => {
    const response = await api.get<Meta>(`/metas/${id}`)
    return response.data
  },

  getMetaProgresso: async (id: string): Promise<MetaProgresso> => {
    const response = await api.get<MetaProgresso>(`/metas/${id}/progresso`)
    return response.data
  },

  createMeta: async (data: CreateMetaInput): Promise<Meta> => {
    const response = await api.post<Meta>('/metas', data)
    return response.data
  },

  updateMeta: async (id: string, data: UpdateMetaInput): Promise<Meta> => {
    const response = await api.put<Meta>(`/metas/${id}`, data)
    return response.data
  },

  deleteMeta: async (id: string): Promise<void> => {
    await api.delete(`/metas/${id}`)
  },

  listAlertas: async (): Promise<AlertaOrcamentoDashboard[]> => {
    const response = await api.get<AlertaOrcamentoDashboard[]>('/metas/alertas-orcamento')
    return response.data
  },
}
