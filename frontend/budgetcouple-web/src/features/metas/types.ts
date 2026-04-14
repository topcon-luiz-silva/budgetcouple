export type MetaTipo = 'ECONOMIA' | 'REDUCAO_CATEGORIA';

export interface Meta {
  id: string;
  tipo: MetaTipo;
  nome: string;
  descricao?: string;
  valorAlvo: number;
  valorAtual: number;
  dataInicio: string; // ISO date
  dataTermino: string; // ISO date
  dataLimite?: string; // ISO date (legacy, use dataTermino)
  categoriaId?: string;
  periodo?: string; // MENSAL, TRIMESTRAL, ANUAL
  percentualAlerta: number;
  ativa: boolean;
  criadoEm: string; // ISO datetime
  atualizadoEm: string; // ISO datetime
}

export interface MetaProgresso {
  metaId: string;
  tipo: MetaTipo;
  valorAtual: number;
  valorAlvo: number;
  percentualProgresso: number;
  diasRestantes?: number;
  atingiuAlerta: boolean;
  atingida: boolean;
}

export interface AlertaOrcamentoDashboard {
  metaId: string;
  nomeMeta: string;
  categoriaNome?: string;
  valorAtual: number;
  valorAlvo: number;
  percentualUtilizado: number;
}

export interface CreateMetaInput {
  tipo: MetaTipo;
  nome: string;
  descricao?: string;
  valorAlvo: number;
  dataInicio: string;
  dataTermino: string;
  dataLimite?: string;
  categoriaId?: string;
  periodo?: string;
  percentualAlerta?: number;
}

export interface UpdateMetaInput {
  id: string;
  tipo: MetaTipo;
  nome: string;
  descricao?: string;
  valorAlvo: number;
  dataInicio: string;
  dataTermino: string;
  dataLimite?: string;
  categoriaId?: string;
  periodo?: string;
  percentualAlerta?: number;
}
