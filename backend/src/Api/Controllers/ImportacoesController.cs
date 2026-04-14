namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Import.Commands;
using BudgetCouple.Application.Import.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Endpoints for importing transactions from OFX and CSV files.
/// </summary>
[ApiController]
[Route("api/v1/importacoes")]
[Tags("Importações")]
public class ImportacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ImportacoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Preview imported transactions without saving them.
    /// Detects duplicates and suggests categories based on rules and history.
    /// </summary>
    [HttpPost("preview")]
    [ProducesResponseType(typeof(ImportPreviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportPreviewDto>> PreviewImport(
        [FromQuery] Guid? contaId,
        [FromQuery] Guid? cartaoId,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Arquivo não fornecido");
        }

        if (!contaId.HasValue && !cartaoId.HasValue)
        {
            return BadRequest("ContaId ou CartaoId deve ser informado");
        }

        using var stream = file.OpenReadStream();
        var command = new PreviewImportCommand
        {
            FileStream = stream,
            FileName = file.FileName,
            ContaId = contaId,
            CartaoId = cartaoId
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error.Message);
    }

    /// <summary>
    /// Confirm import and create transactions as REALIZADA.
    /// </summary>
    [HttpPost("confirmar")]
    [ProducesResponseType(typeof(ConfirmImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConfirmImportResultDto>> ConfirmImport(
        [FromBody] ConfirmImportDto request,
        CancellationToken cancellationToken)
    {
        if (!request.ContaId.HasValue && !request.CartaoId.HasValue)
        {
            return BadRequest("ContaId ou CartaoId deve ser informado");
        }

        var command = new ConfirmImportCommand
        {
            ContaId = request.ContaId,
            CartaoId = request.CartaoId,
            Lancamentos = request.Lancamentos
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error.Message);
    }
}
