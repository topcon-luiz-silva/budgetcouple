namespace BudgetCouple.Application.Notifications.Queries;

using BudgetCouple.Application.Notifications.Dtos;
using BudgetCouple.Application.Notifications.Interfaces;
using MediatR;

public record ObterPreferenciasQuery : IRequest<NotificationPreferencesDto>;

public class ObterPreferenciasHandler : IRequestHandler<ObterPreferenciasQuery, NotificationPreferencesDto>
{
    private readonly INotificationPreferencesRepository _repository;

    public ObterPreferenciasHandler(INotificationPreferencesRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationPreferencesDto> Handle(ObterPreferenciasQuery request, CancellationToken cancellationToken)
    {
        var preferences = await _repository.GetOrCreateAsync(cancellationToken);

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
