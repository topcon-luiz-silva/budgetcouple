namespace BudgetCouple.Application.Classification.Commands;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using MediatR;

public class DeleteRuleHandler : IRequestHandler<DeleteRuleCommand, Result>
{
    private readonly IRegraClassificacaoRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    public DeleteRuleHandler(IRegraClassificacaoRepository repository, IApplicationDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteRuleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var rule = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (rule == null)
            {
                return Result.Failure(
                    Error.NotFound("Regra de classificação não encontrada"));
            }

            _repository.Delete(rule);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Validation(ex.Message));
        }
    }
}
