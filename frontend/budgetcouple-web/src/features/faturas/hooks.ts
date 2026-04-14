import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { faturasApi } from './api'
import type { PayFaturaData } from './types'

const FATURAS_QUERY_KEY = ['faturas']

export function useFaturasList(cartaoId: string) {
  return useQuery({
    queryKey: [FATURAS_QUERY_KEY, 'list', cartaoId],
    queryFn: () => faturasApi.listFaturas(cartaoId),
    enabled: !!cartaoId,
  })
}

export function useFatura(cartaoId: string, competencia: string) {
  return useQuery({
    queryKey: [FATURAS_QUERY_KEY, 'detail', cartaoId, competencia],
    queryFn: () => faturasApi.getFatura(cartaoId, competencia),
    enabled: !!cartaoId && !!competencia,
  })
}

export function usePagarFatura(cartaoId: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ competencia, body }: { competencia: string; body: PayFaturaData }) =>
      faturasApi.pagarFatura(cartaoId, competencia, body),
    onSuccess: (fatura) => {
      queryClient.invalidateQueries({ queryKey: [FATURAS_QUERY_KEY, 'list', cartaoId] })
      queryClient.invalidateQueries({
        queryKey: [FATURAS_QUERY_KEY, 'detail', cartaoId, fatura.competencia],
      })
    },
  })
}
