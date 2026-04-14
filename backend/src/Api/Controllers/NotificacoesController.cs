namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Notifications.Commands;
using BudgetCouple.Application.Notifications.Dtos;
using BudgetCouple.Application.Notifications.Queries;
using BudgetCouple.Domain.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/notificacoes")]
[Authorize]
public class NotificacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificacoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("preferencias")]
    public async Task<ActionResult<NotificationPreferencesDto>> ObterPreferencias(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObterPreferenciasQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("preferencias")]
    public async Task<ActionResult<NotificationPreferencesDto>> AtualizarPreferencias(
        NotificationPreferencesDto preferences,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AtualizarPreferenciasCommand(preferences), cancellationToken);
        return Ok(result);
    }

    [HttpPost("test")]
    public async Task<ActionResult<bool>> EnviarNotificacaoTeste(
        [FromQuery] string canal,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<NotificationChannel>(canal, ignoreCase: true, out var canalEnum))
        {
            return BadRequest("Canal invalido. Use: Email, WebPush ou Telegram");
        }

        var result = await _mediator.Send(new EnviarNotificacaoTesteCommand(canalEnum), cancellationToken);
        return Ok(result);
    }

    [HttpGet("historico")]
    public async Task<ActionResult<List<NotificationHistoryDto>>> ObterHistorico(
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ObterHistoricoQuery(limit), cancellationToken);
        return Ok(result);
    }

    [HttpPost("webpush/subscribe")]
    public async Task<ActionResult<bool>> SubscribeWebPush(
        [FromBody] WebPushSubscriptionDto subscription,
        CancellationToken cancellationToken)
    {
        // TODO: Store subscription in database for later use
        // For now, just acknowledge receipt
        return Ok(true);
    }
}

public class WebPushSubscriptionDto
{
    public string? Endpoint { get; set; }
    public KeysDto? Keys { get; set; }
}

public class KeysDto
{
    public string? Auth { get; set; }
    public string? P256dh { get; set; }
}
