namespace BudgetCouple.Application.Common.Interfaces.Accounting;

using BudgetCouple.Domain.Accounting.Contas;

/// <summary>
/// Repository interface for Conta aggregate.
/// </summary>
public interface IContaRepository
{
    Task<Conta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Conta>> ListAsync(CancellationToken cancellationToken = default);
    void Add(Conta conta);
    void Update(Conta conta);
    void Delete(Conta conta);
}
