namespace BudgetCouple.Application.Common.Interfaces.Budgeting;

using BudgetCouple.Domain.Budgeting.Metas;

public interface IMetaRepository
{
    Task AddAsync(Meta meta, CancellationToken cancellationToken = default);
    Task<Meta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Meta>> ListAsync(CancellationToken cancellationToken = default);
    void Remove(Meta meta);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
