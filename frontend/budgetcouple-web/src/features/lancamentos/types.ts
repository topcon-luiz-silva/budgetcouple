import { z } from 'zod'

export const TipoLancamento = {
  RECEITA: 'RECEITA',
  DESPESA: 'DESPESA',
  TRANSFERENCIA: 'TRANSFERENCIA',
} as const

export type TipoLancamento = typeof TipoLancamento[keyof typeof TipoLancamento]

export const NaturezaLancamento = {
  DESPESA: 'DESPESA',
  RECEITA: 'RECEITA',
  TRANSFERENCIA: 'TRANSFERENCIA',
} as const

export type NaturezaLancamento =
  typeof NaturezaLancamento[keyof typeof NaturezaLancamento]

export const StatusPagamento = {
  PREVISTO: 'PREVISTO',
  REALIZADO: 'REALIZADO',
  ATRASADO: 'ATRASADO',
} as const

export type StatusPagamento =
  typeof StatusPagamento[keyof typeof StatusPagamento]

export const FrequenciaRecorrencia = {
  DIARIA: 'DIARIA',
  SEMANAL: 'SEMANAL',
  QUINZENAL: 'QUINZENAL',
  MENSAL: 'MENSAL',
  TRIMESTRAL: 'TRIMESTRAL',
  SEMESTRAL: 'SEMESTRAL',
  ANUAL: 'ANUAL',
} as const

export type FrequenciaRecorrencia =
  typeof FrequenciaRecorrencia[keyof typeof FrequenciaRecorrencia]

export interface Lancamento {
  id: string
  descricao: string
  valor: number
  dataCompetencia: string
  dataVencimento?: string
  dataPagamento?: string
  naturezaLancamento: NaturezaLancamento
  statusPagamento: StatusPagamento
  contaId?: string
  cartaoId?: string
  categoriaId: string
  subcategoriaId?: string
  tags: string[]
  observacoes?: string
  parceladoId?: string
  recorrenciaId?: string
  numeroMês?: number
  totalMeses?: number
  criadoEm: string
  atualizadoEm: string
}

export interface ListaLancamentosResponse {
  items: Lancamento[]
  total: number
  skip: number
  take: number
}

export interface Recorrencia {
  id: string
  descricaoBase: string
  valorBase: number
  frequencia: FrequenciaRecorrencia
  dataInicio: string
  dataFim?: string
  naturezaLancamento: NaturezaLancamento
  contaId?: string
  cartaoId?: string
  categoriaId: string
  subcategoriaId?: string
  tags: string[]
  observacoes?: string
  ativa: boolean
  criadoEm: string
  atualizadoEm: string
}

// Zod Schemas

export const lancamentoSimplesFormSchema = z
  .object({
    descricao: z.string().min(2, 'Descrição deve ter no mínimo 2 caracteres').max(200),
    valor: z.coerce.number().positive('Valor deve ser positivo'),
    dataCompetencia: z.string(),
    dataVencimento: z.string().optional(),
    naturezaLancamento: z.enum(['DESPESA', 'RECEITA', 'TRANSFERENCIA']),
    contaId: z.string().uuid().optional(),
    cartaoId: z.string().uuid().optional(),
    categoriaId: z.string().uuid(),
    subcategoriaId: z.string().uuid().optional(),
    tags: z.array(z.string()).max(10),
    observacoes: z.string().max(500).optional(),
    statusPagamento: z.enum(['PREVISTO', 'REALIZADO']),
    dataPagamento: z.string().optional(),
  })
  .refine((data) => data.contaId || data.cartaoId, {
    message: 'Você deve selecionar uma conta ou um cartão',
    path: ['contaId'],
  })
  .refine(
    (data) => {
      if (data.statusPagamento === 'REALIZADO') {
        return !!data.dataPagamento
      }
      return true
    },
    {
      message: 'Data de pagamento é obrigatória para pagamento realizado',
      path: ['dataPagamento'],
    }
  )

export type LancamentoSimplesFormData = z.infer<typeof lancamentoSimplesFormSchema>

export const lancamentoParceladoFormSchema = z.object({
  descricaoBase: z.string().min(2, 'Descrição deve ter no mínimo 2 caracteres').max(200),
  valorTotal: z.coerce.number().positive('Valor deve ser positivo'),
  totalParcelas: z.coerce.number().int().min(2, 'Mínimo 2 parcelas').max(99, 'Máximo 99 parcelas'),
  dataPrimeiraParcela: z.string(),
  naturezaLancamento: z.enum(['DESPESA', 'RECEITA', 'TRANSFERENCIA']),
  contaId: z.string().uuid().optional(),
  cartaoId: z.string().uuid().optional(),
  categoriaId: z.string().uuid(),
  subcategoriaId: z.string().uuid().optional(),
  tags: z.array(z.string()).max(10),
  observacoes: z.string().max(500).optional(),
})
  .refine((data) => data.contaId || data.cartaoId, {
    message: 'Você deve selecionar uma conta ou um cartão',
    path: ['contaId'],
  })

export type LancamentoParceladoFormData = z.infer<typeof lancamentoParceladoFormSchema>

export const lancamentoRecorrenteFormSchema = z.object({
  descricaoBase: z.string().min(2, 'Descrição deve ter no mínimo 2 caracteres').max(200),
  valorBase: z.coerce.number().positive('Valor deve ser positivo'),
  frequencia: z.enum(['DIARIA', 'SEMANAL', 'QUINZENAL', 'MENSAL', 'TRIMESTRAL', 'SEMESTRAL', 'ANUAL']),
  dataInicio: z.string(),
  dataFim: z.string().optional(),
  naturezaLancamento: z.enum(['DESPESA', 'RECEITA', 'TRANSFERENCIA']),
  contaId: z.string().uuid().optional(),
  cartaoId: z.string().uuid().optional(),
  categoriaId: z.string().uuid(),
  subcategoriaId: z.string().uuid().optional(),
  tags: z.array(z.string()).max(10),
  observacoes: z.string().max(500).optional(),
  gerarOcorrenciasAte: z.string().optional(),
})
  .refine((data) => data.contaId || data.cartaoId, {
    message: 'Você deve selecionar uma conta ou um cartão',
    path: ['contaId'],
  })

export type LancamentoRecorrenteFormData = z.infer<typeof lancamentoRecorrenteFormSchema>
