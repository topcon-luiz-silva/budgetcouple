import { api } from '@/lib/api'
import type {
  Lancamento,
  ListaLancamentosResponse,
  LancamentoSimplesFormData,
  LancamentoParceladoFormData,
  LancamentoRecorrenteFormData,
  Recorrencia,
} from './types'

export interface ListaLancamentosParams {
  dataInicio?: string
  dataFim?: string
  contaId?: string
  cartaoId?: string
  categoriaId?: string
  status?: string
  tipo?: string
  naturezaLancamento?: string
  skip?: number
  take?: number
}

export const lancamentosApi = {
  list: async (params: ListaLancamentosParams): Promise<ListaLancamentosResponse> => {
    const { data } = await api.get<ListaLancamentosResponse>('/lancamentos', { params })
    return data
  },

  getById: async (id: string): Promise<Lancamento> => {
    const { data } = await api.get<Lancamento>(`/lancamentos/${id}`)
    return data
  },

  createSimples: async (input: LancamentoSimplesFormData): Promise<Lancamento> => {
    const { data } = await api.post<Lancamento>('/lancamentos/simples', input)
    return data
  },

  createParcelado: async (input: LancamentoParceladoFormData): Promise<Lancamento> => {
    const { data } = await api.post<Lancamento>('/lancamentos/parcelado', input)
    return data
  },

  createRecorrencia: async (input: LancamentoRecorrenteFormData): Promise<Recorrencia> => {
    const { data } = await api.post<Recorrencia>('/lancamentos/recorrencia', input)
    return data
  },

  update: async (id: string, input: LancamentoSimplesFormData): Promise<Lancamento> => {
    const { data } = await api.put<Lancamento>(`/lancamentos/${id}`, input)
    return data
  },

  delete: async (id: string, escopo: 'one' | 'fromHere' | 'all' = 'one'): Promise<void> => {
    await api.delete(`/lancamentos/${id}`, { params: { escopo } })
  },

  pagar: async (id: string, dataPagamento: string, contaDebitoId?: string): Promise<Lancamento> => {
    const { data } = await api.post<Lancamento>(`/lancamentos/${id}/pagar`, {
      dataPagamento,
      contaDebitoId,
    })
    return data
  },

  // Recorrências
  listRecorrencias: async (): Promise<Recorrencia[]> => {
    const { data } = await api.get<Recorrencia[]>('/recorrencias')
    return data
  },

  getRecorrenciaById: async (id: string): Promise<Recorrencia> => {
    const { data } = await api.get<Recorrencia>(`/recorrencias/${id}`)
    return data
  },

  updateRecorrencia: async (id: string, input: LancamentoRecorrenteFormData): Promise<Recorrencia> => {
    const { data } = await api.put<Recorrencia>(`/recorrencias/${id}`, input)
    return data
  },

  deleteRecorrencia: async (id: string): Promise<void> => {
    await api.delete(`/recorrencias/${id}`)
  },

  gerarOcorrenciasRecorrencia: async (id: string, ate: string): Promise<Lancamento[]> => {
    const { data } = await api.post<Lancamento[]>(`/recorrencias/${id}/gerar-ocorrencias`, { ate })
    return data
  },
}
