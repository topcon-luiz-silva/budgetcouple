namespace BudgetCouple.Infrastructure.Repositories;

using BudgetCouple.Application.Notifications.Interfaces;
using BudgetCouple.Domain.Notifications;
using BudgetCouple.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class NotificationHistoryRepository : INotificationHistoryRepository
{
    private readonly AppDbContext _dbContext;

    public NotificationHistoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(NotificationHistory history, CancellationToken cancellationToken = default)
    {
        await _dbContext.NotificationHistory.AddAsync(history, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<NotificationHistory>> GetRecentAsync(int limit = 20, CancellationToken cancellationToken = default)
    {
        return await _dbContext.NotificationHistory
            .OrderByDescending(x => x.EnviadoEm)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
