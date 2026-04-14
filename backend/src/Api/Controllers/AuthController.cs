namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public AuthController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns whether PIN has been configured.
    /// </summary>
    [HttpGet("status")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatus()
    {
        var appConfig = await _dbContext.AppConfigs.FirstOrDefaultAsync();
        var pinConfigured = appConfig != null && !string.IsNullOrEmpty(appConfig.PinHash);

        return Ok(new { pinConfigured });
    }

    /// <summary>
    /// Setup PIN (not yet implemented).
    /// </summary>
    [HttpPost("setup-pin")]
    [AllowAnonymous]
    public IActionResult SetupPin()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Not implemented in Phase 1");
    }

    /// <summary>
    /// Login with PIN (not yet implemented).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Not implemented in Phase 1");
    }

    /// <summary>
    /// Change PIN (not yet implemented).
    /// </summary>
    [HttpPost("change-pin")]
    [Authorize]
    public IActionResult ChangePin()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Not implemented in Phase 1");
    }
}
