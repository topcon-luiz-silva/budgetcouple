namespace BudgetCouple.Infrastructure.Services.Notifications;

using BudgetCouple.Application.Notifications.Interfaces;
using BudgetCouple.Domain.Notifications;
using Microsoft.Extensions.Logging;

public class CompositeNotificationService : INotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IWebPushSender _webPushSender;
    private readonly ITelegramSender _telegramSender;
    private readonly INotificationPreferencesRepository _preferencesRepository;
    private readonly INotificationHistoryRepository _historyRepository;
    private readonly ILogger<CompositeNotificationService> _logger;

    public CompositeNotificationService(
        IEmailSender emailSender,
        IWebPushSender webPushSender,
        ITelegramSender telegramSender,
        INotificationPreferencesRepository preferencesRepository,
        INotificationHistoryRepository historyRepository,
        ILogger<CompositeNotificationService> logger)
    {
        _emailSender = emailSender;
        _webPushSender = webPushSender;
        _telegramSender = telegramSender;
        _preferencesRepository = preferencesRepository;
        _historyRepository = historyRepository;
        _logger = logger;
    }

    public async Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        var preferences = await _preferencesRepository.GetAsync(cancellationToken);
        if (preferences == null)
        {
            _logger.LogWarning("Notification preferences not found");
            return;
        }

        var tasks = new List<Task>();

        // Send via Email
        if (preferences.EmailHabilitado && !string.IsNullOrEmpty(preferences.EmailEndereco))
        {
            tasks.Add(SendEmailAsync(preferences.EmailEndereco, message, cancellationToken));
        }

        // Send via WebPush
        if (preferences.WebPushHabilitado)
        {
            tasks.Add(SendWebPushAsync(message, cancellationToken));
        }

        // Send via Telegram
        if (preferences.TelegramHabilitado && !string.IsNullOrEmpty(preferences.TelegramChatId))
        {
            tasks.Add(SendTelegramAsync(message, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    private async Task SendEmailAsync(string emailAddress, NotificationMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await _emailSender.SendAsync(emailAddress, message.Titulo, message.Corpo, cancellationToken);

            var history = NotificationHistory.Create(
                NotificationChannel.Email,
                message.Tipo,
                message.Titulo,
                message.Corpo,
                NotificationStatus.Success
            );
            await _historyRepository.AddAsync(history, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notification");
            var history = NotificationHistory.Create(
                NotificationChannel.Email,
                message.Tipo,
                message.Titulo,
                message.Corpo,
                NotificationStatus.Failed,
                ex.Message
            );
            await _historyRepository.AddAsync(history, cancellationToken);
        }
    }

    private async Task SendWebPushAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await _webPushSender.SendAsync(message.Titulo, message.Corpo, message.Link, cancellationToken);

            var history = NotificationHistory.Create(
                NotificationChannel.WebPush,
                message.Tipo,
                message.Titulo,
                message.Corpo,
                NotificationStatus.Success
            );
            await _historyRepository.AddAsync(history, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending web push notification");
            var history = NotificationHistory.Create(
                NotificationChannel.WebPush,
                message.Tipo,
                message.Titulo,
                message.Corpo,
                NotificationStatus.Failed,
                ex.Message
            );
            await _historyRepository.AddAsync(history, cancellationToken);
        }
    }

    private async Task SendTelegramAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await _telegramSender.SendAsync(message.Titulo, message.Corpo, cancellationToken);

            var history = NotificationHistory.Create(
                NotificationChannel.Telegram,
                message.Tipo,
                message.Titulo,
                message.Corpo,
                NotificationStatus.Success
            );
            await _historyRepository.AddAsync(history, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Telegram notification");
            var history = NotificationHistory.Create(
                NotificationChannel.Telegram,
                message.Tipo,
                message.Titulo,
                message.Corpo,
                NotificationStatus.Failed,
                ex.Message
            );
            await _historyRepository.AddAsync(history, cancellationToken);
        }
    }
}
