namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Delete;

using BudgetCouple.Domain.Common;
using MediatR;

public record DeleteLancamentoCommand(
    Guid Id,
    string? Escopo = null) : IRequest<Result>;
