namespace BudgetCouple.Application.Common.Interfaces.Accounting;

using BudgetCouple.Domain.Accounting.Cartoes;

/// <summary>
/// Repository interface for Cartao aggregate.
/// </summary>
public interface ICartaoRepository
{
    Task<Cartao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Cartao>> ListAsync(CancellationToken cancellationToken = default);
    void Add(Cartao cartao);
    void Update(Cartao cartao);
    void Delete(Cartao cartao);
}
