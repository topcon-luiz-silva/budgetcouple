import { api } from '@/lib/api'
import { z } from 'zod'
import type { Cartao, CartaoFormData } from './types'

// Response Validation Schema - with passthrough to allow extra fields from backend
const cartaoSchema = z.object({
  id: z.string(),
  nome: z.string(),
  bandeira: z.string(),
  ultimosDigitos: z.string().optional(),
  limite: z.number(),
  diaFechamento: z.number(),
  diaVencimento: z.number(),
  contaPagamentoId: z.string(),
  corHex: z.string(),
  icone: z.string(),
  criadoEm: z.string(),
  atualizadoEm: z.string(),
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

export const cartoesApi = {
  list: async (): Promise<Cartao[]> => {
    const { data } = await api.get<Cartao[]>('/cartoes')
    return validateResponse(data, z.array(cartaoSchema), 'cartoesApi.list')
  },

  getById: async (id: string): Promise<Cartao> => {
    const { data } = await api.get<Cartao>(`/cartoes/${id}`)
    return validateResponse(data, cartaoSchema, `cartoesApi.getById(${id})`)
  },

  create: async (input: CartaoFormData): Promise<Cartao> => {
    const { data } = await api.post<Cartao>('/cartoes', input)
    return validateResponse(data, cartaoSchema, 'cartoesApi.create')
  },

  update: async (id: string, input: CartaoFormData): Promise<Cartao> => {
    const { data } = await api.put<Cartao>(`/cartoes/${id}`, input)
    return validateResponse(data, cartaoSchema, `cartoesApi.update(${id})`)
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/cartoes/${id}`)
  },
}
