namespace BudgetCouple.Infrastructure.Persistence.Repositories;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Identity;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for AppConfig aggregate.
/// </summary>
public class AppConfigRepository : IAppConfigRepository
{
    private readonly AppDbContext _dbContext;

    public AppConfigRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppConfig?> GetSingleAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AppConfigs.FirstOrDefaultAsync(cancellationToken);
    }

    public void Add(AppConfig appConfig)
    {
        _dbContext.AppConfigs.Add(appConfig);
    }

    public void Update(AppConfig appConfig)
    {
        _dbContext.AppConfigs.Update(appConfig);
    }
}
