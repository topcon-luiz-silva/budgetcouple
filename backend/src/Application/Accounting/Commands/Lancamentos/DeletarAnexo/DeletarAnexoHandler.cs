namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.DeletarAnexo;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using MediatR;

public class DeletarAnexoHandler : IRequestHandler<DeletarAnexoCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILancamentoAnexoRepository _anexoRepository;
    private readonly IAttachmentStorage _storage;

    public DeletarAnexoHandler(
        IApplicationDbContext dbContext,
        ILancamentoAnexoRepository anexoRepository,
        IAttachmentStorage storage)
    {
        _dbContext = dbContext;
        _anexoRepository = anexoRepository;
        _storage = storage;
    }

    public async Task Handle(DeletarAnexoCommand request, CancellationToken cancellationToken)
    {
        // Get anexo
        var anexo = await _anexoRepository.GetByIdAsync(request.AnexoId, cancellationToken);
        if (anexo == null || anexo.LancamentoId != request.LancamentoId)
            throw new InvalidOperationException($"Anexo {request.AnexoId} não encontrado.");

        // Delete from storage
        await _storage.DeleteAsync(anexo.CaminhoStorage, cancellationToken);

        // Delete from database
        _anexoRepository.Delete(anexo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
