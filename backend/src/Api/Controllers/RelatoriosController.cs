namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Dashboard.Queries.ExportDashboard;
using BudgetCouple.Application.Dashboard.Queries.ExportLancamentos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly IMediator _mediator;

    public RelatoriosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Export lancamentos as Excel
    /// </summary>
    [HttpGet("lancamentos/excel")]
    public async Task<IActionResult> ExportLancamentosExcel(
        [FromQuery] DateOnly? dataInicio = null,
        [FromQuery] DateOnly? dataFim = null,
        [FromQuery] Guid? contaId = null,
        [FromQuery] Guid? cartaoId = null,
        [FromQuery] Guid? categoriaId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportLancamentosExcelQuery(dataInicio, dataFim, contaId, cartaoId, categoriaId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return File(
            result.Value,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "lancamentos.xlsx");
    }

    /// <summary>
    /// Export lancamentos as PDF
    /// </summary>
    [HttpGet("lancamentos/pdf")]
    public async Task<IActionResult> ExportLancamentosPdf(
        [FromQuery] DateOnly? dataInicio = null,
        [FromQuery] DateOnly? dataFim = null,
        [FromQuery] Guid? contaId = null,
        [FromQuery] Guid? cartaoId = null,
        [FromQuery] Guid? categoriaId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportLancamentosPdfQuery(dataInicio, dataFim, contaId, cartaoId, categoriaId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return File(result.Value, "application/pdf", "lancamentos.pdf");
    }

    /// <summary>
    /// Export dashboard as PDF
    /// </summary>
    [HttpGet("dashboard/pdf")]
    public async Task<IActionResult> ExportDashboardPdf(
        [FromQuery] string mes,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportDashboardPdfQuery(mes);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return File(result.Value, "application/pdf", $"dashboard-{mes}.pdf");
    }
}
