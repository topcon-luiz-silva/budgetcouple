namespace BudgetCouple.Application.Classification.Queries;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class ListRulesHandler : IRequestHandler<ListRulesQuery, Result<List<RegraClassificacaoDto>>>
{
    private readonly IRegraClassificacaoRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    public ListRulesHandler(IRegraClassificacaoRepository repository, IApplicationDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<List<RegraClassificacaoDto>>> Handle(ListRulesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var rules = request.ApenasAtivas == true
                ? await _repository.ListAtivasAsync(cancellationToken)
                : await _repository.ListAsync(cancellationToken);

            var dbContext = _dbContext as Microsoft.EntityFrameworkCore.DbContext;
            var categorias = await dbContext.Set<BudgetCouple.Domain.Accounting.Categorias.Categoria>()
                .ToListAsync(cancellationToken);

            var subcategorias = await dbContext.Set<BudgetCouple.Domain.Accounting.Categorias.Subcategoria>()
                .ToListAsync(cancellationToken);

            var dtos = rules.Select(r => new RegraClassificacaoDto
            {
                Id = r.Id,
                Nome = r.Nome,
                Padrao = r.Padrao,
                TipoPadrao = r.TipoPadrao.ToString(),
                CategoriaId = r.CategoriaId,
                CategoriaNome = categorias.FirstOrDefault(c => c.Id == r.CategoriaId)?.Nome,
                SubcategoriaId = r.SubcategoriaId,
                SubcategoriaNome = r.SubcategoriaId.HasValue
                    ? subcategorias.FirstOrDefault(sc => sc.Id == r.SubcategoriaId)?.Nome
                    : null,
                Prioridade = r.Prioridade,
                Ativa = r.Ativa,
                CriadoEm = r.CriadoEm,
                AtualizadoEm = r.AtualizadoEm
            }).ToList();

            return Result.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<RegraClassificacaoDto>>(
                Error.Validation(ex.Message));
        }
    }
}
