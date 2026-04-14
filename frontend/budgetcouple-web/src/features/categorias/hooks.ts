import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { categoriasApi } from './api'
import type { CategoriaFormData } from './types'

const CATEGORIAS_QUERY_KEY = ['categorias']

export function useCategoriasList() {
  return useQuery({
    queryKey: CATEGORIAS_QUERY_KEY,
    queryFn: () => categoriasApi.list(),
  })
}

export function useListCategorias() {
  return useQuery({
    queryKey: CATEGORIAS_QUERY_KEY,
    queryFn: () => categoriasApi.list(),
  })
}

export function useCategoriaById(id: string) {
  return useQuery({
    queryKey: [CATEGORIAS_QUERY_KEY, id],
    queryFn: () => categoriasApi.getById(id),
    enabled: !!id,
  })
}

export function useCreateCategoria() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: CategoriaFormData) => categoriasApi.create(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORIAS_QUERY_KEY })
    },
  })
}

export function useUpdateCategoria(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: CategoriaFormData) => categoriasApi.update(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORIAS_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: [CATEGORIAS_QUERY_KEY, id] })
    },
  })
}

export function useDeleteCategoria() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => categoriasApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CATEGORIAS_QUERY_KEY })
    },
  })
}
