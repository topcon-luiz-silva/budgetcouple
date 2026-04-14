namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Accounting.Commands.Cartoes.CreateCartao;
using BudgetCouple.Application.Accounting.Commands.Cartoes.DeleteCartao;
using BudgetCouple.Application.Accounting.Commands.Cartoes.UpdateCartao;
using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Accounting.Queries.Cartoes.GetCartaoById;
using BudgetCouple.Application.Accounting.Queries.Cartoes.ListCartoes;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CartoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List all cartoes.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CartaoDto>>> ListCartoes()
    {
        var query = new ListCartoesQuery();
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get a cartao by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CartaoDto>> GetCartao(Guid id)
    {
        var query = new GetCartaoByIdQuery(id);
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Create a new cartao.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CartaoDto>> CreateCartao([FromBody] CreateCartaoRequest request)
    {
        var command = new CreateCartaoCommand(
            request.Nome,
            request.Bandeira,
            request.UltimosDigitos,
            request.Limite,
            request.DiaFechamento,
            request.DiaVencimento,
            request.ContaPagamentoId,
            request.CorHex,
            request.Icone);

        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCartao), new { id = result.Value.Id }, result.Value)
            : ToActionResult(result);
    }

    /// <summary>
    /// Update a cartao.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CartaoDto>> UpdateCartao(Guid id, [FromBody] UpdateCartaoRequest request)
    {
        var command = new UpdateCartaoCommand(
            id,
            request.Nome,
            request.Bandeira,
            request.UltimosDigitos,
            request.Limite,
            request.DiaFechamento,
            request.DiaVencimento,
            request.ContaPagamentoId,
            request.CorHex,
            request.Icone);

        var result = await _mediator.Send(command);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete a cartao.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCartao(Guid id)
    {
        var command = new DeleteCartaoCommand(id);
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

public record CreateCartaoRequest(
    string Nome,
    string Bandeira,
    string? UltimosDigitos,
    decimal Limite,
    int DiaFechamento,
    int DiaVencimento,
    Guid ContaPagamentoId,
    string CorHex,
    string? Icone);

public record UpdateCartaoRequest(
    string Nome,
    string Bandeira,
    string? UltimosDigitos,
    decimal Limite,
    int DiaFechamento,
    int DiaVencimento,
    Guid ContaPagamentoId,
    string CorHex,
    string? Icone);
