namespace BudgetCouple.Application.Common.Interfaces.Accounting;

using BudgetCouple.Domain.Accounting.Lancamentos;

public interface ILancamentoAnexoRepository
{
    Task AddAsync(LancamentoAnexo anexo, CancellationToken cancellationToken = default);
    Task<LancamentoAnexo?> GetByIdAsync(Guid anexoId, CancellationToken cancellationToken = default);
    Task<List<LancamentoAnexo>> GetByLancamentoIdAsync(Guid lancamentoId, CancellationToken cancellationToken = default);
    void Delete(LancamentoAnexo anexo);
}
