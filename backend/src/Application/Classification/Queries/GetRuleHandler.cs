namespace BudgetCouple.Application.Classification.Queries;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetRuleHandler : IRequestHandler<GetRuleQuery, Result<RegraClassificacaoDto>>
{
    private readonly IRegraClassificacaoRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    public GetRuleHandler(IRegraClassificacaoRepository repository, IApplicationDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<RegraClassificacaoDto>> Handle(GetRuleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var rule = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (rule == null)
            {
                return Result.Failure<RegraClassificacaoDto>(
                    Error.NotFound("Regra de classificação não encontrada"));
            }

            var dbContext = _dbContext as Microsoft.EntityFrameworkCore.DbContext;
            var categoria = await dbContext.Set<BudgetCouple.Domain.Accounting.Categorias.Categoria>()
                .FirstOrDefaultAsync(c => c.Id == rule.CategoriaId, cancellationToken);

            var subcategoria = rule.SubcategoriaId.HasValue
                ? await dbContext.Set<BudgetCouple.Domain.Accounting.Categorias.Subcategoria>()
                    .FirstOrDefaultAsync(sc => sc.Id == rule.SubcategoriaId, cancellationToken)
                : null;

            var dto = new RegraClassificacaoDto
            {
                Id = rule.Id,
                Nome = rule.Nome,
                Padrao = rule.Padrao,
                TipoPadrao = rule.TipoPadrao.ToString(),
                CategoriaId = rule.CategoriaId,
                CategoriaNome = categoria?.Nome,
                SubcategoriaId = rule.SubcategoriaId,
                SubcategoriaNome = subcategoria?.Nome,
                Prioridade = rule.Prioridade,
                Ativa = rule.Ativa,
                CriadoEm = rule.CriadoEm,
                AtualizadoEm = rule.AtualizadoEm
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<RegraClassificacaoDto>(
                Error.Validation(ex.Message));
        }
    }
}
