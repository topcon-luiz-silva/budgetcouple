namespace BudgetCouple.Application.Common.Interfaces.Accounting;

using BudgetCouple.Domain.Accounting.Recorrencias;

/// <summary>
/// Repository interface for Recorrencia aggregate.
/// </summary>
public interface IRecorrenciaRepository
{
    Task<Recorrencia?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Recorrencia>> ListAsync(CancellationToken cancellationToken = default);
    void Add(Recorrencia recorrencia);
    void Update(Recorrencia recorrencia);
    void Delete(Recorrencia recorrencia);
}
