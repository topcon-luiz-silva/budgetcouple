import { z } from 'zod'

export interface LancamentoDto {
  id: string
  descricao: string
  valor: number
  dataCompetencia: string
  categoriaId: string
  categoriaNome: string
  criadoEm: string
  atualizadoEm: string
}

export interface FaturaResumo {
  competencia: string
  valorTotal: number
  paga: boolean
  dataVencimento: string
}

export interface Fatura {
  cartaoId: string
  cartaoNome: string
  competencia: string
  dataFechamento: string
  dataVencimento: string
  valorTotal: number
  paga: boolean
  dataPagamento?: string
  lancamentos: LancamentoDto[]
}

export const payFaturaSchema = z.object({
  dataPagamento: z.string(),
  contaDebitoId: z.string().uuid().optional().or(z.literal('')),
})

export type PayFaturaData = z.infer<typeof payFaturaSchema>
