namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Pagar;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record PagarLancamentoCommand(
    Guid Id,
    DateOnly DataPagamento,
    Guid? ContaDebitoId) : IRequest<Result<LancamentoDto>>;
