namespace BudgetCouple.Application.Dashboard.DTOs;

public record DashboardDto(
    string Competencia,
    ResumoDto Resumo,
    List<CategoriaResumoDto> PorCategoria,
    List<ContaResumoDto> PorConta,
    List<CartaoResumoDto> PorCartao,
    List<EvolucaoMesDto> EvolucaoMensal,
    List<AlertaOrcamentoDto> AlertasOrcamento,
    List<VencimentoProximoDto> ProximosVencimentos);

public record ResumoDto(
    decimal Receitas,
    decimal Despesas,
    decimal Saldo,
    decimal ReceitasPrevisto,
    decimal DespesasPrevisto,
    decimal SaldoPrevisto,
    decimal SaldoConsolidado);

public record CategoriaResumoDto(
    Guid CategoriaId,
    string CategoriaNome,
    string CorHex,
    decimal Total,
    decimal TotalReceitas,
    decimal Percentual);

public record ContaResumoDto(
    Guid ContaId,
    string ContaNome,
    decimal SaldoAtual,
    decimal Entradas,
    decimal Saidas);

public record CartaoResumoDto(
    Guid CartaoId,
    string CartaoNome,
    decimal FaturaBruta,
    decimal Limite,
    decimal Utilizado);

public record EvolucaoMesDto(
    string Mes,
    decimal Receitas,
    decimal Despesas,
    decimal Saldo);

public record AlertaOrcamentoDto(
    string CategoriaNome,
    decimal ValorGasto,
    decimal Limite,
    decimal PercentualUtilizado);

public record VencimentoProximoDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    DateOnly DataVencimento);
