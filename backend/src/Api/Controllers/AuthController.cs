namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Auth.Commands.ChangePin;
using BudgetCouple.Application.Auth.Commands.Login;
using BudgetCouple.Application.Auth.Commands.SetupPin;
using BudgetCouple.Application.Auth.DTOs;
using BudgetCouple.Application.Auth.Queries.GetAuthStatus;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns whether PIN has been configured.
    /// </summary>
    [HttpGet("status")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthStatusDto>> GetStatus()
    {
        var query = new GetAuthStatusQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Setup PIN (first-time configuration).
    /// </summary>
    [HttpPost("setup-pin")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResult>> SetupPin([FromBody] SetupPinRequest request)
    {
        var command = new SetupPinCommand(request.Pin);
        var result = await _mediator.Send(command);
        return ToActionResult(result);
    }

    /// <summary>
    /// Login with PIN.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Pin);
        var result = await _mediator.Send(command);
        return ToActionResult(result);
    }

    /// <summary>
    /// Change PIN (requires authentication).
    /// </summary>
    [HttpPost("change-pin")]
    [Authorize]
    public async Task<IActionResult> ChangePin([FromBody] ChangePinRequest request)
    {
        var command = new ChangePinCommand(request.PinAtual, request.NovoPin);
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok("PIN alterado com sucesso")
            : StatusCode(MapErrorToStatusCode(result.Error), result.Error.Message);
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

// Request DTOs
public record SetupPinRequest(string Pin);
public record LoginRequest(string Pin);
public record ChangePinRequest(string PinAtual, string NovoPin);
