namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Accounting.Commands.Contas.CreateConta;
using BudgetCouple.Application.Accounting.Commands.Contas.DeleteConta;
using BudgetCouple.Application.Accounting.Commands.Contas.UpdateConta;
using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Accounting.Queries.Contas.GetContaById;
using BudgetCouple.Application.Accounting.Queries.Contas.ListContas;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ContasController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List all contas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ContaDto>>> ListContas()
    {
        var query = new ListContasQuery();
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get a conta by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ContaDto>> GetConta(Guid id)
    {
        var query = new GetContaByIdQuery(id);
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Create a new conta.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ContaDto>> CreateConta([FromBody] CreateContaRequest request)
    {
        var command = new CreateContaCommand(
            request.Nome,
            request.TipoConta,
            request.SaldoInicial,
            request.CorHex,
            request.Icone,
            request.Observacoes);

        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetConta), new { id = result.Value.Id }, result.Value)
            : ToActionResult(result);
    }

    /// <summary>
    /// Update a conta.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ContaDto>> UpdateConta(Guid id, [FromBody] UpdateContaRequest request)
    {
        var command = new UpdateContaCommand(id, request.Nome, request.CorHex, request.Icone, request.Observacoes);
        var result = await _mediator.Send(command);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete a conta.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConta(Guid id)
    {
        var command = new DeleteContaCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? NoContent()
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

public record CreateContaRequest(
    string Nome,
    string TipoConta,
    decimal SaldoInicial,
    string CorHex,
    string? Icone,
    string? Observacoes);

public record UpdateContaRequest(
    string Nome,
    string CorHex,
    string? Icone,
    string? Observacoes);
