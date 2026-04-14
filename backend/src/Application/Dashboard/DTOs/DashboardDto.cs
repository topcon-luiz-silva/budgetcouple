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
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal Saldo,
    decimal TotalPrevistoReceitas,
    decimal TotalPrevistoDespesas,
    decimal SaldoConsolidadoContas);

public record CategoriaResumoDto(
    Guid CategoriaId,
    string CategoriaNome,
    string CorHex,
    decimal TotalDespesas,
    decimal TotalReceitas,
    decimal Percentual);

public record ContaResumoDto(
    Guid ContaId,
    string ContaNome,
    decimal Saldo,
    decimal TotalEntradas,
    decimal TotalSaidas);

public record CartaoResumoDto(
    Guid CartaoId,
    string CartaoNome,
    decimal ValorFatura,
    decimal Limite,
    decimal LimiteUtilizadoPct);

public record EvolucaoMesDto(
    string Competencia,
    decimal Receitas,
    decimal Despesas,
    decimal Saldo);

public record AlertaOrcamentoDto(
    Guid CategoriaId,
    string CategoriaNome,
    decimal ValorGasto,
    decimal Limite,
    decimal PercentualUtilizado);

public record VencimentoProximoDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    DateOnly DataVencimento);
