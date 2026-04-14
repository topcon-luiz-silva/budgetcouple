namespace BudgetCouple.Application.Dashboard.DTOs;

public record RelatorioLancamentoDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    DateOnly Data,
    string Tipo,
    string Natureza,
    string Status,
    string? CategoriaNome,
    string? ContaNome,
    string? CartaoNome,
    DateOnly? DataPagamento);
