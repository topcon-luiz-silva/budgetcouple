export type TipoPadrao = 'CONTEM' | 'IGUAL' | 'REGEX' | 'COMECA_COM' | 'TERMINA_COM'

export interface RegraClassificacao {
  id: string
  nome: string
  padrao: string
  tipoPadrao: TipoPadrao
  categoriaId: string
  categoriaNome: string
  subcategoriaId?: string | null
  subcategoriaNome?: string | null
  prioridade: number
  ativa: boolean
}

export interface RegraFormData {
  nome: string
  padrao: string
  tipoPadrao: TipoPadrao
  categoriaId: string
  subcategoriaId?: string | null
  prioridade: number
  ativa: boolean
}
