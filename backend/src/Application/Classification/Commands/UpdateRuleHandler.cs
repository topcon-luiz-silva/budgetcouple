namespace BudgetCouple.Application.Classification.Commands;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using BudgetCouple.Domain.Imports;
using MediatR;

public class UpdateRuleHandler : IRequestHandler<UpdateRuleCommand, Result>
{
    private readonly IRegraClassificacaoRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    public UpdateRuleHandler(IRegraClassificacaoRepository repository, IApplicationDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(UpdateRuleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var rule = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (rule == null)
            {
                return Result.Failure(
                    Error.NotFound("Regra de classificação não encontrada"));
            }

            // Parse TipoPadrao
            if (!Enum.TryParse<TipoPadrao>(request.TipoPadrao, true, out var tipoPadrao))
            {
                return Result.Failure(
                    Error.Validation($"TipoPadrao inválido: {request.TipoPadrao}"));
            }

            // Update rule
            var updateResult = rule.Atualizar(
                nome: request.Nome,
                padrao: request.Padrao,
                tipoPadrao: tipoPadrao,
                categoriaId: request.CategoriaId,
                subcategoriaId: request.SubcategoriaId,
                prioridade: request.Prioridade,
                ativa: request.Ativa);

            if (updateResult.IsFailure)
            {
                return Result.Failure(updateResult.Error);
            }

            _repository.Update(rule);
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
