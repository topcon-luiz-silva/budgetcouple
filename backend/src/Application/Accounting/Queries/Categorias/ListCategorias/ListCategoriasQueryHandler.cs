namespace BudgetCouple.Application.Accounting.Queries.Categorias.ListCategorias;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class ListCategoriasQueryHandler : IRequestHandler<ListCategoriasQuery, Result<List<CategoriaDto>>>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public ListCategoriasQueryHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Result<List<CategoriaDto>>> Handle(ListCategoriasQuery request, CancellationToken cancellationToken)
    {
        var categorias = await _categoriaRepository.ListAsync(cancellationToken);
        var dtos = categorias.Select(c => CategoriaDto.FromDomain(c)).ToList();
        return Result.Success(dtos);
    }
}
