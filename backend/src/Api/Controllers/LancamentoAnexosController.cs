namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Accounting.Commands.Lancamentos.AdicionarAnexo;
using BudgetCouple.Application.Accounting.Commands.Lancamentos.DeletarAnexo;
using BudgetCouple.Application.Accounting.Queries.Lancamentos.GetAnexos;
using BudgetCouple.Application.Accounting.Queries.Lancamentos.GetAnexoById;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Common;
using BudgetCouple.Infrastructure.FileStorage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/lancamentos/{lancamentoId:guid}/anexos")]
[Authorize]
public class LancamentoAnexosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAttachmentStorage _storage;

    public LancamentoAnexosController(IMediator mediator, IAttachmentStorage storage)
    {
        _mediator = mediator;
        _storage = storage;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AdicionarAnexoResponse>> Upload(
        Guid lancamentoId,
        IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Arquivo é obrigatório.");

        const long maxFileSize = 10 * 1024 * 1024; // 10 MB
        if (arquivo.Length > maxFileSize)
            return BadRequest($"Arquivo não pode exceder {maxFileSize / 1024 / 1024} MB.");

        var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
        if (!allowedMimeTypes.Contains(arquivo.ContentType))
            return BadRequest("Tipo de arquivo não permitido. Aceitar: imagens (JPG, PNG, GIF) e PDF.");

        try
        {
            using var stream = arquivo.OpenReadStream();
            var command = new AdicionarAnexoCommand(
                lancamentoId,
                arquivo.FileName,
                arquivo.ContentType,
                arquivo.Length,
                stream);

            var result = await _mediator.Send(command);
            return Created($"/api/v1/lancamentos/{lancamentoId}/anexos/{result.AnexoId}", result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao fazer upload: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<AnexoDto>>> List(Guid lancamentoId)
    {
        var query = new GetAnexosQuery(lancamentoId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpDelete("{anexoId:guid}")]
    public async Task<ActionResult> Delete(Guid lancamentoId, Guid anexoId)
    {
        try
        {
            var command = new DeletarAnexoCommand(lancamentoId, anexoId);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{anexoId:guid}/download")]
    public async Task<ActionResult> Download(Guid lancamentoId, Guid anexoId)
    {
        try
        {
            var query = new GetAnexoByIdQuery(lancamentoId, anexoId);
            var anexo = await _mediator.Send(query);

            var stream = await _storage.GetAsync(anexo.CaminhoStorage);
            return File(stream, anexo.ContentType, anexo.NomeArquivo);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Arquivo não encontrado no storage.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao fazer download: {ex.Message}");
        }
    }
}
