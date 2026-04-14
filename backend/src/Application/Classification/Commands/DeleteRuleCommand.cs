namespace BudgetCouple.Application.Classification.Commands;

using BudgetCouple.Domain.Common;
using MediatR;

public class DeleteRuleCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
