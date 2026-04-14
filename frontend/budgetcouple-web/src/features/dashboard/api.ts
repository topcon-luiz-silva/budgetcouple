import { api } from '@/lib/api'
import type { DashboardDto } from './types'

export const dashboardApi = {
  getDashboard: async (mes: string): Promise<DashboardDto> => {
    const { data } = await api.get<DashboardDto>('/dashboard', {
      params: { mes },
    })
    return data
  },

  exportDashboardPdf: async (mes: string): Promise<Blob> => {
    const { data } = await api.get('/relatorios/dashboard/pdf', {
      params: { mes },
      responseType: 'blob',
    })
    return data
  },

  exportLancamentosExcel: async (params: {
    dataInicio?: string
    dataFim?: string
    contaId?: string
    cartaoId?: string
    categoriaId?: string
    status?: string
  }): Promise<Blob> => {
    const { data } = await api.get('/relatorios/lancamentos/excel', {
      params,
      responseType: 'blob',
    })
    return data
  },

  exportLancamentosPdf: async (params: {
    dataInicio?: string
    dataFim?: string
    contaId?: string
    cartaoId?: string
    categoriaId?: string
    status?: string
  }): Promise<Blob> => {
    const { data } = await api.get('/relatorios/lancamentos/pdf', {
      params,
      responseType: 'blob',
    })
    return data
  },
}
