import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { regrasApi } from './api'
import type { RegraFormData } from './types'

const REGRAS_QUERY_KEY = ['regras-classificacao']

export function useRegrasList(apenasAtivas?: boolean) {
  return useQuery({
    queryKey: [REGRAS_QUERY_KEY, { apenasAtivas }],
    queryFn: () => regrasApi.list(apenasAtivas),
  })
}

export function useRegraById(id: string) {
  return useQuery({
    queryKey: [REGRAS_QUERY_KEY, id],
    queryFn: () => regrasApi.getById(id),
    enabled: !!id,
  })
}

export function useCreateRegra() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: RegraFormData) => regrasApi.create(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: REGRAS_QUERY_KEY })
    },
  })
}

export function useUpdateRegra(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: RegraFormData) => regrasApi.update(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: REGRAS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: [REGRAS_QUERY_KEY, id] })
    },
  })
}

export function useDeleteRegra() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => regrasApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: REGRAS_QUERY_KEY })
    },
  })
}
