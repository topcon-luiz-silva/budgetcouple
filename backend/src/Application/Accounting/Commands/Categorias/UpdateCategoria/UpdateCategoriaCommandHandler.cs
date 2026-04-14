namespace BudgetCouple.Application.Accounting.Commands.Categorias.UpdateCategoria;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class UpdateCategoriaCommandHandler : IRequestHandler<UpdateCategoriaCommand, Result<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _dbContext;

    public UpdateCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IApplicationDbContext dbContext)
    {
        _categoriaRepository = categoriaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<CategoriaDto>> Handle(UpdateCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (categoria == null)
            return Result.Failure<CategoriaDto>(Error.NotFound("Categoria não encontrada"));

        var result = categoria.Renomear(request.Nome);
        if (!result.IsSuccess)
            return Result.Failure<CategoriaDto>(result.Error);

        // Update other fields directly
        var updateProperty = categoria.GetType().GetProperty("Cor");
        if (updateProperty != null && updateProperty.CanWrite)
        {
            updateProperty.SetValue(categoria, request.CorHex);
        }

        var updateIcone = categoria.GetType().GetProperty("Icone");
        if (updateIcone != null && updateIcone.CanWrite)
        {
            updateIcone.SetValue(categoria, request.Icone);
        }

        _categoriaRepository.Update(categoria);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = CategoriaDto.FromDomain(categoria);
        return Result.Success(dto);
    }
}
