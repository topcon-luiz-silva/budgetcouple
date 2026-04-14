import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { cartoesApi } from './api'
import type { CartaoFormData } from './types'

const CARTOES_QUERY_KEY = ['cartoes']

export function useCartoesList() {
  return useQuery({
    queryKey: CARTOES_QUERY_KEY,
    queryFn: () => cartoesApi.list(),
  })
}

export function useCartaoById(id: string) {
  return useQuery({
    queryKey: [CARTOES_QUERY_KEY, id],
    queryFn: () => cartoesApi.getById(id),
    enabled: !!id,
  })
}

export function useCreateCartao() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: CartaoFormData) => cartoesApi.create(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CARTOES_QUERY_KEY })
    },
  })
}

export function useUpdateCartao(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: CartaoFormData) => cartoesApi.update(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CARTOES_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: [CARTOES_QUERY_KEY, id] })
    },
  })
}

export function useDeleteCartao() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => cartoesApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CARTOES_QUERY_KEY })
    },
  })
}
