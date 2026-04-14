namespace BudgetCouple.Infrastructure.Repositories;

using BudgetCouple.Application.Notifications.Interfaces;
using BudgetCouple.Domain.Notifications;
using BudgetCouple.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class NotificationPreferencesRepository : INotificationPreferencesRepository
{
    private readonly AppDbContext _dbContext;

    public NotificationPreferencesRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NotificationPreferences?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.NotificationPreferences.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<NotificationPreferences> GetOrCreateAsync(CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(cancellationToken);
        if (existing != null)
            return existing;

        var newPreferences = NotificationPreferences.Create();
        await AddAsync(newPreferences, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return newPreferences;
    }

    public async Task AddAsync(NotificationPreferences preferences, CancellationToken cancellationToken = default)
    {
        await _dbContext.NotificationPreferences.AddAsync(preferences, cancellationToken);
    }

    public async Task UpdateAsync(NotificationPreferences preferences, CancellationToken cancellationToken = default)
    {
        _dbContext.NotificationPreferences.Update(preferences);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
