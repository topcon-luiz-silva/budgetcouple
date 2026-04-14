namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Dashboard.Queries.GetDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get dashboard data for a specific month (aggregated in one call)
    /// </summary>
    /// <param name="mes">Month in format YYYY-MM (e.g., 2026-04)</param>
    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] string mes,
        CancellationToken cancellationToken)
    {
        var query = new GetDashboardQuery(mes);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(result.Value);
    }
}
