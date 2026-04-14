namespace BudgetCouple.Application.Classification.Queries;

using BudgetCouple.Domain.Common;
using MediatR;

public class GetRuleQuery : IRequest<Result<RegraClassificacaoDto>>
{
    public Guid Id { get; set; }
}
