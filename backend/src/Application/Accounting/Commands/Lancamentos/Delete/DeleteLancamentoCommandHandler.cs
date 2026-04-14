namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Delete;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class DeleteLancamentoCommandHandler : IRequestHandler<DeleteLancamentoCommand, Result>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly IApplicationDbContext _dbContext;

    public DeleteLancamentoCommandHandler(
        ILancamentoRepository lancamentoRepository,
        IApplicationDbContext dbContext)
    {
        _lancamentoRepository = lancamentoRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteLancamentoCommand request, CancellationToken cancellationToken)
    {
        // Get lancamento
        var lancamento = await _lancamentoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lancamento == null)
            return Result.Failure(Error.NotFound($"Lançamento com ID {request.Id} não encontrado"));

        var escopo = request.Escopo ?? "one";

        // If not parcelado, just delete
        if (!lancamento.IsParcelada)
        {
            _lancamentoRepository.Delete(lancamento);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        // Handle parcelado deletion
        var paiId = lancamento.DadosParcelamento?.LancamentoPaiId ?? lancamento.Id;
        var lancamentos = await _lancamentoRepository.GetByParcelacaoAsync(paiId, cancellationToken);

        if (escopo == "one")
        {
            // Delete only this one
            _lancamentoRepository.Delete(lancamento);
        }
        else if (escopo == "fromHere")
        {
            // Delete this and all after it
            var dataThis = lancamento.Data;
            var toDelete = lancamentos.Where(l => l.Data >= dataThis).ToList();
            foreach (var l in toDelete)
                _lancamentoRepository.Delete(l);
        }
        else if (escopo == "all")
        {
            // Delete all parceladas
            foreach (var l in lancamentos)
                _lancamentoRepository.Delete(l);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
