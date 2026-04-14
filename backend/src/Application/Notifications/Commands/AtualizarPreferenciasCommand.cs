namespace BudgetCouple.Application.Notifications.Commands;

using BudgetCouple.Application.Notifications.Dtos;
using BudgetCouple.Application.Notifications.Interfaces;
using MediatR;

public record AtualizarPreferenciasCommand(NotificationPreferencesDto Preferences) : IRequest<NotificationPreferencesDto>;

public class AtualizarPreferenciasHandler : IRequestHandler<AtualizarPreferenciasCommand, NotificationPreferencesDto>
{
    private readonly INotificationPreferencesRepository _repository;

    public AtualizarPreferenciasHandler(INotificationPreferencesRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationPreferencesDto> Handle(AtualizarPreferenciasCommand request, CancellationToken cancellationToken)
    {
        var preferences = await _repository.GetOrCreateAsync(cancellationToken);

        preferences.AtualizarEmailPreferences(request.Preferences.EmailHabilitado, request.Preferences.EmailEndereco);
        preferences.AtualizarWebPushPreferences(request.Preferences.WebPushHabilitado);
        preferences.AtualizarTelegramPreferences(request.Preferences.TelegramHabilitado, request.Preferences.TelegramChatId);
        preferences.AtualizarTiposNotificacao(
            request.Preferences.NotificarVencimentos1Dia,
            request.Preferences.NotificarVencimentosDia,
            request.Preferences.NotificarAlertasOrcamento,
            request.Preferences.NotificarFaturas
        );

        await _repository.UpdateAsync(preferences, cancellationToken);

        return new NotificationPreferencesDto
        {
            EmailHabilitado = preferences.EmailHabilitado,
            EmailEndereco = preferences.EmailEndereco,
            WebPushHabilitado = preferences.WebPushHabilitado,
            TelegramHabilitado = preferences.TelegramHabilitado,
            TelegramChatId = preferences.TelegramChatId,
            NotificarVencimentos1Dia = preferences.NotificarVencimentos1Dia,
            NotificarVencimentosDia = preferences.NotificarVencimentosDia,
            NotificarAlertasOrcamento = preferences.NotificarAlertasOrcamento,
            NotificarFaturas = preferences.NotificarFaturas
        };
    }
}
