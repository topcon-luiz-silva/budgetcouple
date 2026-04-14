import { useQuery, useMutation } from '@tanstack/react-query'
import { dashboardApi } from './api'

const DASHBOARD_QUERY_KEY = ['dashboard']

export function useDashboard(mes: string) {
  return useQuery({
    queryKey: [DASHBOARD_QUERY_KEY, mes],
    queryFn: () => dashboardApi.getDashboard(mes),
    enabled: !!mes,
  })
}

export function useExportDashboardPdf() {
  return useMutation({
    mutationFn: (mes: string) => dashboardApi.exportDashboardPdf(mes),
  })
}

export function useExportLancamentosExcel() {
  return useMutation({
    mutationFn: (params: {
      dataInicio?: string
      dataFim?: string
      contaId?: string
      cartaoId?: string
      categoriaId?: string
      status?: string
    }) => dashboardApi.exportLancamentosExcel(params),
  })
}

export function useExportLancamentosPdf() {
  return useMutation({
    mutationFn: (params: {
      dataInicio?: string
      dataFim?: string
      contaId?: string
      cartaoId?: string
      categoriaId?: string
      status?: string
    }) => dashboardApi.exportLancamentosPdf(params),
  })
}
