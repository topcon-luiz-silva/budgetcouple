namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Budgeting.Metas.Commands;
using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Application.Budgeting.Metas.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/metas")]
public class MetasController : ControllerBase
{
    private readonly IMediator _mediator;

    public MetasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List all active metas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MetaDto>>> ListarMetas(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListarMetasQuery(), cancellationToken);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a specific meta by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MetaDto>> ObterMetaPorId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObterMetaPorIdQuery(id), cancellationToken);
        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Get current progress of a meta, including calculated fields.
    /// </summary>
    [HttpGet("{id}/progresso")]
    public async Task<ActionResult<MetaProgressoDto>> ObterMetaProgresso(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObterMetaProgressoQuery(id), cancellationToken);
        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new meta (ECONOMIA or REDUCAO_CATEGORIA).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MetaDto>> CriarMeta(CriarMetaCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return CreatedAtAction(nameof(ObterMetaPorId), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Update an existing meta.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<MetaDto>> AtualizarMeta(Guid id, AtualizarMetaCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new { error = "ID mismatch" });

        var result = await _mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Delete a meta by ID.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirMeta(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ExcluirMetaCommand(id), cancellationToken);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// List active budget alerts for the current month.
    /// </summary>
    [HttpGet("alertas-orcamento")]
    public async Task<ActionResult<List<AlertaOrcamentoDashboardDto>>> ListarAlertasOrcamento(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListarAlertasOrcamentoQuery(), cancellationToken);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(result.Value);
    }
}
