import { z } from 'zod'

export const TipoCategoria = {
  DESPESA: 'DESPESA',
  RECEITA: 'RECEITA',
} as const

export type TipoCategoria = typeof TipoCategoria[keyof typeof TipoCategoria]

export interface Categoria {
  id: string
  nome: string
  tipoCategoria: TipoCategoria
  corHex: string
  icone: string
  parentCategoriaId?: string
  criadoEm: string
  atualizadoEm: string
  sistema?: boolean
  ativa?: boolean
}

export const categoriaFormSchema = z.object({
  nome: z.string().min(2, 'Nome deve ter no mínimo 2 caracteres').max(100),
  tipoCategoria: z.enum(['DESPESA', 'RECEITA']),
  corHex: z.string().regex(/^#[0-9A-Fa-f]{6}$/, 'Cor deve ser um hexadecimal válido'),
  icone: z.string().min(1, 'Ícone é obrigatório'),
  parentCategoriaId: z.string().uuid('Categoria pai inválida').optional().or(z.literal('')),
})

export type CategoriaFormData = z.infer<typeof categoriaFormSchema>
