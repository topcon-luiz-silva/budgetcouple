namespace BudgetCouple.Application.Accounting.Queries.Lancamentos.GetById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record GetLancamentoByIdQuery(Guid Id) : IRequest<Result<LancamentoDto>>;
