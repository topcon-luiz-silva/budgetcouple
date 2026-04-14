import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { contasApi } from './api'
import type { ContaFormData } from './types'

const CONTAS_QUERY_KEY = ['contas']

export function useContasList() {
  return useQuery({
    queryKey: CONTAS_QUERY_KEY,
    queryFn: () => contasApi.list(),
  })
}

export function useContaById(id: string) {
  return useQuery({
    queryKey: [CONTAS_QUERY_KEY, id],
    queryFn: () => contasApi.getById(id),
    enabled: !!id,
  })
}

export function useCreateConta() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: ContaFormData) => contasApi.create(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CONTAS_QUERY_KEY })
    },
  })
}

export function useUpdateConta(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: ContaFormData) => contasApi.update(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CONTAS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: [CONTAS_QUERY_KEY, id] })
    },
  })
}

export function useDeleteConta() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => contasApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CONTAS_QUERY_KEY })
    },
  })
}
