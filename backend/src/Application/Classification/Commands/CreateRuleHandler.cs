namespace BudgetCouple.Application.Classification.Commands;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using BudgetCouple.Domain.Imports;
using MediatR;

public class CreateRuleHandler : IRequestHandler<CreateRuleCommand, Result<Guid>>
{
    private readonly IRegraClassificacaoRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    public CreateRuleHandler(IRegraClassificacaoRepository repository, IApplicationDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(CreateRuleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Parse TipoPadrao
            if (!Enum.TryParse<TipoPadrao>(request.TipoPadrao, true, out var tipoPadrao))
            {
                return Result.Failure<Guid>(
                    Error.Validation($"TipoPadrao inválido: {request.TipoPadrao}"));
            }

            // Create rule
            var ruleResult = RegraClassificacao.Create(
                nome: request.Nome,
                padrao: request.Padrao,
                tipoPadrao: tipoPadrao,
                categoriaId: request.CategoriaId,
                subcategoriaId: request.SubcategoriaId,
                prioridade: request.Prioridade,
                ativa: request.Ativa);

            if (ruleResult.IsFailure)
            {
                return Result.Failure<Guid>(ruleResult.Error);
            }

            var rule = ruleResult.Value;
            _repository.Add(rule);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(rule.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(
                Error.Validation(ex.Message));
        }
    }
}
