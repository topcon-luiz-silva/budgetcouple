namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Classification.Commands;
using BudgetCouple.Application.Classification.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// CRUD endpoints for classification rules.
/// </summary>
[ApiController]
[Route("api/v1/regras-classificacao")]
[Tags("Regras de Classificação")]
public class RegrasClassificacaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegrasClassificacaoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List all classification rules with optional filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RegraClassificacaoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RegraClassificacaoDto>>> ListRules(
        [FromQuery] bool? apenasAtivas,
        CancellationToken cancellationToken)
    {
        var query = new ListRulesQuery { ApenasAtivas = apenasAtivas };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error.Message);
    }

    /// <summary>
    /// Get a specific classification rule by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RegraClassificacaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegraClassificacaoDto>> GetRule(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetRuleQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error.Message);
    }

    /// <summary>
    /// Create a new classification rule.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateRule(
        [FromBody] CreateRuleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetRule), new { id = result.Value }, new { id = result.Value })
            : BadRequest(result.Error.Message);
    }

    /// <summary>
    /// Update an existing classification rule.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateRule(
        Guid id,
        [FromBody] UpdateRuleCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.Code == "rule_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a classification rule.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRule(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRuleCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.Code == "rule_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
        }

        return NoContent();
    }
}
