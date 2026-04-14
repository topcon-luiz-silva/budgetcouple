namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.Delete;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class DeleteRecorrenciaCommandHandler : IRequestHandler<DeleteRecorrenciaCommand, Result>
{
    private readonly IRecorrenciaRepository _recorrenciaRepository;
    private readonly IApplicationDbContext _dbContext;

    public DeleteRecorrenciaCommandHandler(
        IRecorrenciaRepository recorrenciaRepository,
        IApplicationDbContext dbContext)
    {
        _recorrenciaRepository = recorrenciaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteRecorrenciaCommand request, CancellationToken cancellationToken)
    {
        var recorrencia = await _recorrenciaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (recorrencia == null)
            return Result.Failure(Error.NotFound($"Recorrência com ID {request.Id} não encontrada"));

        _recorrenciaRepository.Delete(recorrencia);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
