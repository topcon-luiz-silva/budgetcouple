import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { lancamentosApi, type ListaLancamentosParams } from './api'
import type {
  LancamentoSimplesFormData,
  LancamentoParceladoFormData,
  LancamentoRecorrenteFormData,
} from './types'

const LANCAMENTOS_QUERY_KEY = ['lancamentos']
const RECORRENCIAS_QUERY_KEY = ['recorrencias']

export function useLancamentosList(params: ListaLancamentosParams) {
  return useQuery({
    queryKey: [LANCAMENTOS_QUERY_KEY, params],
    queryFn: () => lancamentosApi.list(params),
  })
}

export function useLancamentoById(id: string) {
  return useQuery({
    queryKey: [LANCAMENTOS_QUERY_KEY, id],
    queryFn: () => lancamentosApi.getById(id),
    enabled: !!id,
  })
}

export function useCreateLancamentoSimples() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: LancamentoSimplesFormData) => lancamentosApi.createSimples(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
    },
  })
}

export function useCreateLancamentoParcelado() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: LancamentoParceladoFormData) => lancamentosApi.createParcelado(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
    },
  })
}

export function useCreateLancamentoRecorrencia() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: LancamentoRecorrenteFormData) => lancamentosApi.createRecorrencia(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: RECORRENCIAS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
    },
  })
}

export function useUpdateLancamento(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: LancamentoSimplesFormData) => lancamentosApi.update(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: [LANCAMENTOS_QUERY_KEY, id] })
    },
  })
}

export function useDeleteLancamento() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, escopo }: { id: string; escopo: 'one' | 'fromHere' | 'all' }) =>
      lancamentosApi.delete(id, escopo),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
    },
  })
}

export function usePagarLancamento() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, dataPagamento, contaDebitoId }: { id: string; dataPagamento: string; contaDebitoId?: string }) =>
      lancamentosApi.pagar(id, dataPagamento, contaDebitoId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
    },
  })
}

export function useRecorrenciasList() {
  return useQuery({
    queryKey: RECORRENCIAS_QUERY_KEY,
    queryFn: () => lancamentosApi.listRecorrencias(),
  })
}

export function useRecorrenciaById(id: string) {
  return useQuery({
    queryKey: [RECORRENCIAS_QUERY_KEY, id],
    queryFn: () => lancamentosApi.getRecorrenciaById(id),
    enabled: !!id,
  })
}

export function useUpdateRecorrencia(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: LancamentoRecorrenteFormData) => lancamentosApi.updateRecorrencia(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: RECORRENCIAS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: [RECORRENCIAS_QUERY_KEY, id] })
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
    },
  })
}

export function useDeleteRecorrencia() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => lancamentosApi.deleteRecorrencia(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: RECORRENCIAS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
    },
  })
}

export function useGerarOcorrenciasRecorrencia() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, ate }: { id: string; ate: string }) =>
      lancamentosApi.gerarOcorrenciasRecorrencia(id, ate),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: LANCAMENTOS_QUERY_KEY })
    },
  })
}
