namespace BudgetCouple.Application.Common.Interfaces;

/// <summary>
/// Application database context interface.
/// Infrastructure will implement this with EF Core.
/// </summary>
public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
