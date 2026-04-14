namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/backup")]
[Authorize]
public class BackupController : ControllerBase
{
    private readonly IBackupService _backupService;

    public BackupController(IBackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpGet("export")]
    public async Task<ActionResult> Export(CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonData = await _backupService.ExportAsync(cancellationToken);
            var fileName = $"budgetcouple-backup-{DateTime.UtcNow:yyyy-MM-dd}.json";
            return File(
                System.Text.Encoding.UTF8.GetBytes(jsonData),
                "application/json",
                fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Erro ao exportar backup: {ex.Message}" });
        }
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Import(
        IFormFile arquivo,
        [FromQuery] string modo = "merge",
        CancellationToken cancellationToken = default)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest(new { error = "Arquivo é obrigatório." });

        if (modo != "merge" && modo != "replace")
            return BadRequest(new { error = "Modo deve ser 'merge' ou 'replace'." });

        try
        {
            using var reader = new StreamReader(arquivo.OpenReadStream());
            var jsonData = await reader.ReadToEndAsync(cancellationToken);

            await _backupService.ImportAsync(jsonData, modo, cancellationToken);
            return Ok(new { message = $"Backup importado com sucesso (modo: {modo})." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Erro ao importar backup: {ex.Message}" });
        }
    }
}
