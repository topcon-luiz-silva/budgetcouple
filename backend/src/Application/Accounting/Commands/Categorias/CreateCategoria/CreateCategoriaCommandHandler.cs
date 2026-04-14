namespace BudgetCouple.Application.Accounting.Commands.Categorias.CreateCategoria;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Categorias;
using BudgetCouple.Domain.Common;
using MediatR;

public class CreateCategoriaCommandHandler : IRequestHandler<CreateCategoriaCommand, Result<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _dbContext;

    public CreateCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IApplicationDbContext dbContext)
    {
        _categoriaRepository = categoriaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<CategoriaDto>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<TipoCategoria>(request.TipoCategoria, out var tipoCategoria))
        {
            return Result.Failure<CategoriaDto>(Error.Validation("Tipo de categoria inválido"));
        }

        var result = Categoria.Create(
            request.Nome,
            tipoCategoria,
            request.Icone,
            request.CorHex,
            sistema: false);

        if (!result.IsSuccess)
            return Result.Failure<CategoriaDto>(result.Error);

        var categoria = result.Value;
        _categoriaRepository.Add(categoria);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = CategoriaDto.FromDomain(categoria);
        return Result.Success(dto);
    }
}
