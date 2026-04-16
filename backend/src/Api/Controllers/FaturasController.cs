namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Accounting.Commands.Faturas.EstornarFatura;
using BudgetCouple.Application.Accounting.Commands.Faturas.PagarFatura;
using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Accounting.Queries.Faturas.GetFatura;
using BudgetCouple.Application.Accounting.Queries.Faturas.ListFaturas;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/cartoes/{cartaoId}/faturas")]
[Authorize]
public class FaturasController : ControllerBase
{
    private readonly IMediator _mediator;

    public FaturasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List the last 12 invoices for a credit card (summary).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<FaturaResumoDto>>> List(Guid cartaoId)
    {
        var query = new ListFaturasQuery(cartaoId);
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get a specific invoice by competencia (YYYY-MM).
    /// </summary>
    [HttpGet("{competencia}")]
    public async Task<ActionResult<FaturaDto>> GetFatura(Guid cartaoId, string competencia)
    {
        var query = new GetFaturaQuery(cartaoId, competencia);
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Pay an invoice.
    /// Body: { dataPagamento?: "YYYY-MM-DD", contaDebitoId?: "uuid" }
    /// </summary>
    [HttpPost("{competencia}/pagar")]
    public async Task<ActionResult<FaturaDto>> PagarFatura(
        Guid cartaoId,
        string competencia,
        [FromBody] PagarFaturaRequest request)
    {
        var dataPagamento = request.DataPagamento.HasValue
            ? DateOnly.Parse(request.DataPagamento.Value.ToString("yyyy-MM-dd"))
            : (DateOnly?)null;

        var command = new PagarFaturaCommand(
            cartaoId,
            competencia,
            dataPagamento,
            request.ContaDebitoId);

        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(MapErrorToStatusCode(result.Error), new { error = result.Error.Message });
    }

    /// <summary>
    /// Reverse (estornar) a paid invoice.
    /// </summary>
    [HttpPost("{competencia}/estornar")]
    public async Task<ActionResult<FaturaDto>> EstornarFatura(
        Guid cartaoId,
        string competencia)
    {
        var command = new EstornarFaturaCommand(cartaoId, competencia);
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(MapErrorToStatusCode(result.Error), new { error = result.Error.Message });
    }

    private ActionResult<T> ToActionResult<T>(Result<T> result) =>
        result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(MapErrorToStatusCode(result.Error), new { error = result.Error.Message });

    private int MapErrorToStatusCode(Error error) =>
        error.Code switch
        {
            "NotFound" => StatusCodes.Status404NotFound,
            "Conflict" => StatusCodes.Status409Conflict,
            "Unauthorized" => StatusCodes.Status401Unauthorized,
            "Forbidden" => StatusCodes.Status403Forbidden,
            "Validation" => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
}

public record PagarFaturaRequest(
    DateOnly? DataPagamento = null,
    Guid? ContaDebitoId = null
);
