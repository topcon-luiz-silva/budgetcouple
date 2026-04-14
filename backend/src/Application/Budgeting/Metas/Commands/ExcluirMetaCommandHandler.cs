namespace BudgetCouple.Application.Budgeting.Metas.Commands;

using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Common;
using MediatR;

public class ExcluirMetaCommandHandler : IRequestHandler<ExcluirMetaCommand, Result>
{
    private readonly IMetaRepository _metaRepository;

    public ExcluirMetaCommandHandler(IMetaRepository metaRepository)
    {
        _metaRepository = metaRepository;
    }

    public async Task<Result> Handle(ExcluirMetaCommand request, CancellationToken cancellationToken)
    {
        var meta = await _metaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (meta == null)
            return Result.Failure(Error.NotFound("Meta não encontrada."));

        _metaRepository.Remove(meta);
        await _metaRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
