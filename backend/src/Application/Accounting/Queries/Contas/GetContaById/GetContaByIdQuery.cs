namespace BudgetCouple.Application.Accounting.Queries.Contas.GetContaById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record GetContaByIdQuery(Guid Id) : IRequest<Result<ContaDto>>;
