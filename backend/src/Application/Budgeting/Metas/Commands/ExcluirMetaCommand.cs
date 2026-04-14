namespace BudgetCouple.Application.Budgeting.Metas.Commands;

using BudgetCouple.Domain.Common;
using MediatR;

public record ExcluirMetaCommand(Guid Id) : IRequest<Result>;
