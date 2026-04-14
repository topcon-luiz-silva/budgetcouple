namespace BudgetCouple.Application.Common.Interfaces;

using BudgetCouple.Domain.Identity;

/// <summary>
/// Repository interface for AppConfig aggregate.
/// </summary>
public interface IAppConfigRepository
{
    Task<AppConfig?> GetSingleAsync(CancellationToken cancellationToken = default);
    void Add(AppConfig appConfig);
    void Update(AppConfig appConfig);
}
