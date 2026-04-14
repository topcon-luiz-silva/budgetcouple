namespace BudgetCouple.Application.Accounting.DTOs;

public record FaturaDto(
    Guid CartaoId,
    string CartaoNome,
    string Competencia, // YYYY-MM
    int DataFechamento,
    string DataVencimento, // YYYY-MM-DD
    decimal ValorTotal,
    bool Paga,
    string? DataPagamento, // YYYY-MM-DD
    List<LancamentoDto> Lancamentos
);

public record FaturaResumoDto(
    Guid CartaoId,
    string Competencia, // YYYY-MM
    decimal ValorTotal,
    bool Paga,
    string DataVencimento // YYYY-MM-DD
);
