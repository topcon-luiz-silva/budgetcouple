namespace BudgetCouple.Application.Budgeting.Metas.Queries;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record ObterMetaPorIdQuery(Guid Id) : IRequest<Result<MetaDto>>;
