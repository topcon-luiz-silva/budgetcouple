namespace BudgetCouple.Application.Notifications.Queries;

using BudgetCouple.Application.Notifications.Dtos;
using BudgetCouple.Application.Notifications.Interfaces;
using MediatR;

public record ObterHistoricoQuery(int Limit = 20) : IRequest<List<NotificationHistoryDto>>;

public class ObterHistoricoHandler : IRequestHandler<ObterHistoricoQuery, List<NotificationHistoryDto>>
{
    private readonly INotificationHistoryRepository _repository;

    public ObterHistoricoHandler(INotificationHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<NotificationHistoryDto>> Handle(ObterHistoricoQuery request, CancellationToken cancellationToken)
    {
        var historico = await _repository.GetRecentAsync(request.Limit, cancellationToken);

        return historico.Select(h => new NotificationHistoryDto
        {
            Id = h.Id,
            Canal = h.Canal,
            Tipo = h.Tipo,
            Titulo = h.Titulo,
            Corpo = h.Corpo,
            Status = h.Status,
            Erro = h.Erro,
            EnviadoEm = h.EnviadoEm
        }).ToList();
    }
}
