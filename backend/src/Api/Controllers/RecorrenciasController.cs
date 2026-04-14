namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Accounting.Commands.Recorrencias.Delete;
using BudgetCouple.Application.Accounting.Commands.Recorrencias.GerarOcorrencias;
using BudgetCouple.Application.Accounting.Commands.Recorrencias.Update;
using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Accounting.Queries.Recorrencias.GetById;
using BudgetCouple.Application.Accounting.Queries.Recorrencias.List;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RecorrenciasController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecorrenciasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<RecorrenciaDto>>> List()
    {
        var query = new ListRecorrenciasQuery();
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecorrenciaDto>> GetById(Guid id)
    {
        var query = new GetRecorrenciaByIdQuery(id);
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RecorrenciaDto>> Update(Guid id, [FromBody] UpdateRecorrenciaRequest request)
    {
        var command = new UpdateRecorrenciaCommand(id, request.DataFim);
        var result = await _mediator.Send(command);
        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteRecorrenciaCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? NoContent()
            : StatusCode(MapErrorToStatusCode(result.Error), new { error = result.Error.Message });
    }

    [HttpPost("{id}/gerar-ocorrencias")]
    public async Task<ActionResult<List<LancamentoDto>>> GerarOcorrencias(Guid id, [FromQuery] DateOnly ate)
    {
        var command = new GerarOcorrenciasCommand(id, ate);
        var result = await _mediator.Send(command);
        return ToActionResult(result);
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

public record UpdateRecorrenciaRequest(
    DateOnly? DataFim);
