namespace BudgetCouple.Application.Accounting.Queries.Categorias.GetCategoriaById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class GetCategoriaByIdQueryHandler : IRequestHandler<GetCategoriaByIdQuery, Result<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public GetCategoriaByIdQueryHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Result<CategoriaDto>> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (categoria == null)
            return Result.Failure<CategoriaDto>(Error.NotFound("Categoria não encontrada"));

        var dto = CategoriaDto.FromDomain(categoria);
        return Result.Success(dto);
    }
}
