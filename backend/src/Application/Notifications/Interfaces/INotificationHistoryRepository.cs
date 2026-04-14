namespace BudgetCouple.Application.Notifications.Interfaces;

using BudgetCouple.Domain.Notifications;

public interface INotificationHistoryRepository
{
    Task AddAsync(NotificationHistory history, CancellationToken cancellationToken = default);
    Task<List<NotificationHistory>> GetRecentAsync(int limit = 20, CancellationToken cancellationToken = default);
}
