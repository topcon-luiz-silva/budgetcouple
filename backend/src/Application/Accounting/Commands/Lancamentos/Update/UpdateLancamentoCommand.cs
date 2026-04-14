namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Update;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record UpdateLancamentoCommand(
    Guid Id,
    string Descricao,
    decimal Valor,
    DateOnly DataCompetencia,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    List<string>? Tags) : IRequest<Result<LancamentoDto>>;
