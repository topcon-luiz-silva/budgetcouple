import { z } from 'zod'

export const TipoConta = {
  CONTA_CORRENTE: 'CONTA_CORRENTE',
  POUPANCA: 'POUPANCA',
  INVESTIMENTO: 'INVESTIMENTO',
  CARTEIRA: 'CARTEIRA',
} as const

export type TipoConta = typeof TipoConta[keyof typeof TipoConta]

export interface Conta {
  id: string
  nome: string
  tipoConta: TipoConta
  saldoInicial: number
  corHex: string
  icone: string
  observacoes?: string
  criadoEm?: string
  atualizadoEm?: string
  saldoAtual?: number
  ativa?: boolean
}

export const contaFormSchema = z.object({
  nome: z.string().min(2, 'Nome deve ter no mínimo 2 caracteres').max(100),
  tipoConta: z.enum(['CONTA_CORRENTE', 'POUPANCA', 'INVESTIMENTO', 'CARTEIRA']),
  saldoInicial: z.union([z.number(), z.string()]).pipe(z.coerce.number()),
  corHex: z.string().regex(/^#[0-9A-Fa-f]{6}$/, 'Cor deve ser um hexadecimal válido'),
  icone: z.string().min(1, 'Ícone é obrigatório'),
  observacoes: z.string().max(500).optional(),
})

export type ContaFormData = z.infer<typeof contaFormSchema>
