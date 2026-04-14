namespace BudgetCouple.Application.Common.Interfaces;

using BudgetCouple.Domain.Imports;

/// <summary>
/// Repository interface for RegraClassificacao aggregate.
/// </summary>
public interface IRegraClassificacaoRepository
{
    Task<RegraClassificacao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<RegraClassificacao>> ListAsync(CancellationToken cancellationToken = default);
    Task<List<RegraClassificacao>> ListAtivasAsync(CancellationToken cancellationToken = default);
    void Add(RegraClassificacao regra);
    void Update(RegraClassificacao regra);
    void Delete(RegraClassificacao regra);
}
