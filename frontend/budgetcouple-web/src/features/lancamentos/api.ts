import { api } from '@/lib/api'
import { z } from 'zod'
import type {
  Lancamento,
  ListaLancamentosResponse,
  LancamentoSimplesFormData,
  LancamentoParceladoFormData,
  LancamentoRecorrenteFormData,
  Recorrencia,
} from './types'

// Response Validation Schemas
const lancamentoSchema = z.object({
  id: z.string(),
  descricao: z.string(),
  valor: z.number(),
  dataCompetencia: z.string(),
  dataVencimento: z.string().optional(),
  dataPagamento: z.string().optional(),
  naturezaLancamento: z.enum(['DESPESA', 'RECEITA', 'TRANSFERENCIA']),
  statusPagamento: z.enum(['PREVISTO', 'REALIZADO', 'ATRASADO']),
  contaId: z.string().optional(),
  cartaoId: z.string().optional(),
  categoriaId: z.string(),
  subcategoriaId: z.string().optional(),
  tags: z.array(z.string()).default([]),
  observacoes: z.string().optional(),
  parceladoId: z.string().optional(),
  recorrenciaId: z.string().optional(),
  numeroMês: z.number().optional(),
  totalMeses: z.number().optional(),
  criadoEm: z.string().optional(),
  atualizadoEm: z.string().optional(),
})

const listaLancamentosResponseSchema = z.object({
  items: z.array(lancamentoSchema),
  total: z.number(),
  skip: z.number(),
  take: z.number(),
})

const recorrenciaSchema = z.object({
  id: z.string(),
  descricaoBase: z.string(),
  valorBase: z.number(),
  frequencia: z.enum(['DIARIA', 'SEMANAL', 'QUINZENAL', 'MENSAL', 'TRIMESTRAL', 'SEMESTRAL', 'ANUAL']),
  dataInicio: z.string(),
  dataFim: z.string().optional(),
  naturezaLancamento: z.enum(['DESPESA', 'RECEITA', 'TRANSFERENCIA']),
  contaId: z.string().optional(),
  cartaoId: z.string().optional(),
  categoriaId: z.string(),
  subcategoriaId: z.string().optional(),
  tags: z.array(z.string()).default([]),
  observacoes: z.string().optional(),
  ativa: z.boolean(),
  criadoEm: z.string().optional(),
  atualizadoEm: z.string().optional(),
})

function validateResponse<T>(data: unknown, schema: z.ZodSchema<T>, context: string): T {
  const result = schema.safeParse(data)
  if (!result.success) {
    const message = result.error.issues.map((issue: any) => `${issue.path.join('.')}: ${issue.message}`).join(', ')
    console.warn(`[Contract Test Warning] ${context}: ${message}`)
    return data as T
  }
  return result.data
}

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
    return validateResponse(data, listaLancamentosResponseSchema, 'lancamentosApi.list')
  },

  getById: async (id: string): Promise<Lancamento> => {
    const { data } = await api.get<Lancamento>(`/lancamentos/${id}`)
    return validateResponse(data, lancamentoSchema, `lancamentosApi.getById(${id})`)
  },

  createSimples: async (input: LancamentoSimplesFormData): Promise<Lancamento> => {
    const { data } = await api.post<Lancamento>('/lancamentos/simples', input)
    return validateResponse(data, lancamentoSchema, 'lancamentosApi.createSimples')
  },

  createParcelado: async (input: LancamentoParceladoFormData): Promise<Lancamento> => {
    const { data } = await api.post<Lancamento>('/lancamentos/parcelado', input)
    return validateResponse(data, lancamentoSchema, 'lancamentosApi.createParcelado')
  },

  createRecorrencia: async (input: LancamentoRecorrenteFormData): Promise<Recorrencia> => {
    const { data } = await api.post<Recorrencia>('/lancamentos/recorrencia', input)
    return validateResponse(data, recorrenciaSchema, 'lancamentosApi.createRecorrencia')
  },

  update: async (id: string, input: LancamentoSimplesFormData): Promise<Lancamento> => {
    const { data } = await api.put<Lancamento>(`/lancamentos/${id}`, input)
    return validateResponse(data, lancamentoSchema, `lancamentosApi.update(${id})`)
  },

  delete: async (id: string, escopo: 'one' | 'fromHere' | 'all' = 'one'): Promise<void> => {
    await api.delete(`/lancamentos/${id}`, { params: { escopo } })
  },

  pagar: async (id: string, dataPagamento: string, contaDebitoId?: string): Promise<Lancamento> => {
    const { data } = await api.post<Lancamento>(`/lancamentos/${id}/pagar`, {
      dataPagamento,
      contaDebitoId,
    })
    return validateResponse(data, lancamentoSchema, `lancamentosApi.pagar(${id})`)
  },

  // Recorrências
  listRecorrencias: async (): Promise<Recorrencia[]> => {
    const { data } = await api.get<Recorrencia[]>('/recorrencias')
    return validateResponse(data, z.array(recorrenciaSchema), 'lancamentosApi.listRecorrencias')
  },

  getRecorrenciaById: async (id: string): Promise<Recorrencia> => {
    const { data } = await api.get<Recorrencia>(`/recorrencias/${id}`)
    return validateResponse(data, recorrenciaSchema, `lancamentosApi.getRecorrenciaById(${id})`)
  },

  updateRecorrencia: async (id: string, input: LancamentoRecorrenteFormData): Promise<Recorrencia> => {
    const { data } = await api.put<Recorrencia>(`/recorrencias/${id}`, input)
    return validateResponse(data, recorrenciaSchema, `lancamentosApi.updateRecorrencia(${id})`)
  },

  deleteRecorrencia: async (id: string): Promise<void> => {
    await api.delete(`/recorrencias/${id}`)
  },

  gerarOcorrenciasRecorrencia: async (id: string, ate: string): Promise<Lancamento[]> => {
    const { data } = await api.post<Lancamento[]>(`/recorrencias/${id}/gerar-ocorrencias`, { ate })
    return validateResponse(data, z.array(lancamentoSchema), `lancamentosApi.gerarOcorrenciasRecorrencia(${id})`)
  },
}
