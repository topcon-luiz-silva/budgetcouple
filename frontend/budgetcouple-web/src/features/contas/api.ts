import { api } from '@/lib/api'
import { z } from 'zod'
import type { Conta, ContaFormData } from './types'

// Response Validation Schema - with passthrough to allow extra fields from backend
const contaSchema = z.object({
  id: z.string(),
  nome: z.string(),
  tipoConta: z.enum(['CONTA_CORRENTE', 'POUPANCA', 'INVESTIMENTO', 'CARTEIRA']),
  saldoInicial: z.number(),
  corHex: z.string(),
  icone: z.string(),
  observacoes: z.string().optional(),
  criadoEm: z.string().optional(),
  atualizadoEm: z.string().optional(),
}).passthrough()

function validateResponse<T>(data: unknown, schema: z.ZodSchema<T>, context: string): T {
  try {
    return schema.parse(data)
  } catch (error) {
    const message = error instanceof z.ZodError 
      ? `${context}: ${error.issues.map((issue: any) => `${issue.path.join('.')}: ${issue.message}`).join(', ')}`
      : `${context}: validation failed`
    console.warn(`[Contract Test Warning] ${message}`)
    throw new Error(`API response validation failed: ${message}`)
  }
}

export const contasApi = {
  list: async (): Promise<Conta[]> => {
    const { data } = await api.get<Conta[]>('/contas')
    return validateResponse(data, z.array(contaSchema), 'contasApi.list')
  },

  getById: async (id: string): Promise<Conta> => {
    const { data } = await api.get<Conta>(`/contas/${id}`)
    return validateResponse(data, contaSchema, `contasApi.getById(${id})`)
  },

  create: async (input: ContaFormData): Promise<Conta> => {
    const { data } = await api.post<Conta>('/contas', input)
    return validateResponse(data, contaSchema, 'contasApi.create')
  },

  update: async (id: string, input: ContaFormData): Promise<Conta> => {
    const { data } = await api.put<Conta>(`/contas/${id}`, input)
    return validateResponse(data, contaSchema, `contasApi.update(${id})`)
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/contas/${id}`)
  },
}
