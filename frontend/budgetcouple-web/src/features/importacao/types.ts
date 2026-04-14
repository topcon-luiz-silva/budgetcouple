export interface ImportItemDto {
  descricao: string
  valor: number
  dataCompetencia: string
  categoriaSugeridaId?: string | null
  categoriaSugeridaNome?: string | null
  duplicado: boolean
  hashImportacao: string
}

export interface ImportPreviewDto {
  lancamentos: ImportItemDto[]
}

export interface ConfirmImportItemDto {
  descricao: string
  valor: number
  dataCompetencia: string
  categoriaId?: string | null
  subcategoriaId?: string | null
  duplicado: boolean
  hashImportacao: string
}

export interface ConfirmImportDto {
  contaId?: string | null
  cartaoId?: string | null
  lancamentos: ConfirmImportItemDto[]
}

export interface ConfirmImportResultDto {
  sucesso: number
  falhas: number
  mensagens: string[]
}
