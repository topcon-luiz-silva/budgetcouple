import { api } from '@/lib/api'
import { z } from 'zod'
import type { Categoria, CategoriaFormData } from './types'

// Response Validation Schema - with passthrough to allow extra fields from backend
const categoriaSchema = z.object({
  id: z.string(),
  nome: z.string(),
  tipoCategoria: z.enum(['DESPESA', 'RECEITA']),
  corHex: z.string(),
  icone: z.string(),
  parentCategoriaId: z.string().optional(),
  criadoEm: z.string().optional(),
  atualizadoEm: z.string().optional(),
}).passthrough()

function validateResponse<T>(data: unknown, schema: z.ZodSchema<T>, context: string): T {
  const result = schema.safeParse(data)
  if (!result.success) {
    const message = result.error.issues.map((issue: any) => `${issue.path.join('.')}: ${issue.message}`).join(', ')
    console.warn(`[Contract Test Warning] ${context}: ${message}`)
    return data as T
  }
  return result.data
}

export const categoriasApi = {
  list: async (): Promise<Categoria[]> => {
    const { data } = await api.get<Categoria[]>('/categorias')
    return validateResponse(data, z.array(categoriaSchema), 'categoriasApi.list')
  },

  getById: async (id: string): Promise<Categoria> => {
    const { data } = await api.get<Categoria>(`/categorias/${id}`)
    return validateResponse(data, categoriaSchema, `categoriasApi.getById(${id})`)
  },

  create: async (input: CategoriaFormData): Promise<Categoria> => {
    const { data } = await api.post<Categoria>('/categorias', input)
    return validateResponse(data, categoriaSchema, 'categoriasApi.create')
  },

  update: async (id: string, input: CategoriaFormData): Promise<Categoria> => {
    const { data } = await api.put<Categoria>(`/categorias/${id}`, input)
    return validateResponse(data, categoriaSchema, `categoriasApi.update(${id})`)
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/categorias/${id}`)
  },
}
