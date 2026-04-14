export interface ResumoDto {
  receitas: number
  despesas: number
  saldo: number
  saldoConsolidado: number
  receitasPrevisto: number
  despesasPrevisto: number
  saldoPrevisto: number
}

export interface PorCategoriaItem {
  categoriaId: string
  categoriaNome: string
  corHex: string
  total: number
  percentual: number
}

export interface PorContaItem {
  contaId: string
  contaNome: string
  saldoAtual: number
  entradas: number
  saidas: number
}

export interface PorCartaoItem {
  cartaoId: string
  cartaoNome: string
  faturaBruta: number
  limite: number
  utilizado: number
}

export interface EvolucaoMensalItem {
  mes: string
  receitas: number
  despesas: number
  saldo: number
}

export interface AlertaOrcamento {
  id: string
  categoriaId: string
  categoriaNome: string
  orcamento: number
  gasto: number
  percentualUtilizado: number
}

export interface ProximoVencimento {
  id: string
  descricao: string
  valor: number
  dataVencimento: string
  natureza: 'RECEITA' | 'DESPESA' | 'TRANSFERENCIA'
  categoriaId: string
  categoriaNome: string
}

export interface DashboardDto {
  competencia: string
  resumo: ResumoDto
  porCategoria: PorCategoriaItem[]
  porConta: PorContaItem[]
  porCartao: PorCartaoItem[]
  evolucaoMensal: EvolucaoMensalItem[]
  alertasOrcamento: AlertaOrcamento[]
  proximosVencimentos: ProximoVencimento[]
}
