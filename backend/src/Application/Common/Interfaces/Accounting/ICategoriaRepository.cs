namespace BudgetCouple.Application.Common.Interfaces.Accounting;

using BudgetCouple.Domain.Accounting.Categorias;

/// <summary>
/// Repository interface for Categoria aggregate.
/// </summary>
public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Categoria>> ListAsync(CancellationToken cancellationToken = default);
    Task<Categoria?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default);
    void Add(Categoria categoria);
    void Update(Categoria categoria);
    void Delete(Categoria categoria);
}
