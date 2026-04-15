namespace BudgetCouple.Application.Accounting.DTOs;

public record CreateRecorrenciaResponse(
    RecorrenciaDto Recorrencia,
    List<LancamentoDto> Lancamentos);
