namespace BudgetCouple.Domain.Accounting;

public enum TipoConta { CONTA_CORRENTE, POUPANCA, CARTEIRA, INVESTIMENTO }
public enum TipoCategoria { DESPESA, RECEITA }
public enum TipoLancamento { RECEITA, DESPESA }
public enum NaturezaLancamento { PREVISTA, REALIZADA }
public enum ClassificacaoRecorrencia { FIXA, VARIAVEL }
public enum StatusPagamento { PENDENTE, PAGO, ATRASADO }
public enum FrequenciaRecorrencia { MENSAL, BIMESTRAL, TRIMESTRAL, SEMESTRAL, ANUAL }
