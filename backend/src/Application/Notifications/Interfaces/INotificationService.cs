namespace BudgetCouple.Application.Notifications.Interfaces;

using BudgetCouple.Domain.Notifications;

public class NotificationMessage
{
    public string Titulo { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
    public string? Link { get; set; }
    public NotificationType Tipo { get; set; }
}

public interface INotificationService
{
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}

public interface IEmailSender
{
    Task SendAsync(string emailAddress, string titulo, string corpo, CancellationToken cancellationToken = default);
    Task<bool> IsConfiguredAsync();
}

public interface IWebPushSender
{
    Task SendAsync(string titulo, string corpo, string? link, CancellationToken cancellationToken = default);
    Task<bool> IsConfiguredAsync();
}

public interface ITelegramSender
{
    Task SendAsync(string titulo, string corpo, CancellationToken cancellationToken = default);
    Task<bool> IsConfiguredAsync();
}
