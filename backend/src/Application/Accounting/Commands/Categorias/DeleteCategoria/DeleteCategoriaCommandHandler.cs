namespace BudgetCouple.Application.Accounting.Commands.Categorias.DeleteCategoria;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class DeleteCategoriaCommandHandler : IRequestHandler<DeleteCategoriaCommand, Result>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _dbContext;

    public DeleteCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IApplicationDbContext dbContext)
    {
        _categoriaRepository = categoriaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (categoria == null)
            return Result.Failure(Error.NotFound("Categoria não encontrada"));

        // Prevent deletion of system categories
        if (categoria.Sistema)
            return Result.Failure(Error.Conflict("Categoria do sistema não pode ser excluída"));

        _categoriaRepository.Delete(categoria);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
