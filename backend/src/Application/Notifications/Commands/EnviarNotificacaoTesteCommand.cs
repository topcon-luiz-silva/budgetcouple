namespace BudgetCouple.Application.Notifications.Commands;

using BudgetCouple.Application.Notifications.Interfaces;
using BudgetCouple.Domain.Notifications;
using MediatR;

public record EnviarNotificacaoTesteCommand(NotificationChannel Canal) : IRequest<bool>;

public class EnviarNotificacaoTesteHandler : IRequestHandler<EnviarNotificacaoTesteCommand, bool>
{
    private readonly INotificationService _notificationService;
    private readonly INotificationPreferencesRepository _preferencesRepository;
    private readonly INotificationHistoryRepository _historyRepository;

    public EnviarNotificacaoTesteHandler(
        INotificationService notificationService,
        INotificationPreferencesRepository preferencesRepository,
        INotificationHistoryRepository historyRepository)
    {
        _notificationService = notificationService;
        _preferencesRepository = preferencesRepository;
        _historyRepository = historyRepository;
    }

    public async Task<bool> Handle(EnviarNotificacaoTesteCommand request, CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetAsync(cancellationToken);
        if (preferences == null)
            return false;

        var message = new NotificationMessage
        {
            Titulo = "Notificacao de Teste",
            Corpo = "Esta e uma notificacao de teste para validar a configuracao do canal",
            Tipo = NotificationType.VencimentoProximo
        };

        try
        {
            await _notificationService.SendAsync(message, cancellationToken);

            // Registrar no histórico
            var history = NotificationHistory.Create(
                request.Canal,
                NotificationType.VencimentoProximo,
                message.Titulo,
                message.Corpo,
                NotificationStatus.Success
            );
            await _historyRepository.AddAsync(history, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            // Registrar erro no histórico
            var history = NotificationHistory.Create(
                request.Canal,
                NotificationType.VencimentoProximo,
                message.Titulo,
                message.Corpo,
                NotificationStatus.Failed,
                ex.Message
            );
            await _historyRepository.AddAsync(history, cancellationToken);
            return false;
        }
    }
}
