namespace BudgetCouple.Application.Accounting.Queries.Cartoes.GetCartaoById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record GetCartaoByIdQuery(Guid Id) : IRequest<Result<CartaoDto>>;
