namespace BudgetCouple.Application.Accounting.Queries.Cartoes.ListCartoes;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record ListCartoesQuery : IRequest<Result<List<CartaoDto>>>;
