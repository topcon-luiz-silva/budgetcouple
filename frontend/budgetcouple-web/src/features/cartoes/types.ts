import { z } from 'zod'

export interface Cartao {
  id: string
  nome: string
  bandeira: string
  ultimosDigitos?: string
  limite: number
  diaFechamento: number
  diaVencimento: number
  contaPagamentoId: string
  corHex: string
  icone: string
  criadoEm: string
  atualizadoEm: string
}

export const cartaoFormSchema = z.object({
  nome: z.string().min(2, 'Nome deve ter no mínimo 2 caracteres').max(100),
  bandeira: z.string().min(1, 'Bandeira é obrigatória'),
  ultimosDigitos: z.string().regex(/^\d{4}$/, 'Últimos 4 dígitos').optional().or(z.literal('')),
  limite: z.coerce.number().positive('Limite deve ser positivo'),
  diaFechamento: z.coerce.number().int().min(1).max(31),
  diaVencimento: z.coerce.number().int().min(1).max(31),
  contaPagamentoId: z.string().uuid('Conta de pagamento inválida').or(z.string().min(1)),
  corHex: z.string().regex(/^#[0-9A-Fa-f]{6}$/, 'Cor deve ser um hexadecimal válido'),
  icone: z.string().min(1, 'Ícone é obrigatório'),
})

export type CartaoFormData = z.infer<typeof cartaoFormSchema>
