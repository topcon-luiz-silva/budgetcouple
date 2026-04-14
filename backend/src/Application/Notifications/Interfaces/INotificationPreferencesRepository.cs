namespace BudgetCouple.Application.Notifications.Interfaces;

using BudgetCouple.Domain.Notifications;

public interface INotificationPreferencesRepository
{
    Task<NotificationPreferences?> GetAsync(CancellationToken cancellationToken = default);
    Task<NotificationPreferences> GetOrCreateAsync(CancellationToken cancellationToken = default);
    Task AddAsync(NotificationPreferences preferences, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationPreferences preferences, CancellationToken cancellationToken = default);
}
