namespace BudgetCouple.Application.Accounting.DTOs;

public record ListaLancamentosResponse(
    List<LancamentoDto> Items,
    int Total,
    int Skip,
    int Take);
