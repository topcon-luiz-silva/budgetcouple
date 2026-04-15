import { api } from '@/lib/api'
import type { Fatura, FaturaResumo, PayFaturaData } from './types'

export const faturasApi = {
  listFaturas: async (cartaoId: string): Promise<FaturaResumo[]> => {
    const { data } = await api.get<FaturaResumo[]>(`/cartoes/${cartaoId}/faturas`)
    return data
  },

  getFatura: async (cartaoId: string, competencia: string): Promise<Fatura> => {
    const { data } = await api.get<Fatura>(`/cartoes/${cartaoId}/faturas/${competencia}`)
    return data
  },

  pagarFatura: async (cartaoId: string, competencia: string, body: PayFaturaData): Promise<Fatura> => {
    const { data } = await api.post<Fatura>(
      `/cartoes/${cartaoId}/faturas/${competencia}/pagar`,
      body
    )
    return data
  },
}
