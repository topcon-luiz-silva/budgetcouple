namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Accounting.Commands.Categorias.CreateCategoria;
using BudgetCouple.Application.Accounting.Commands.Categorias.DeleteCategoria;
using BudgetCouple.Application.Accounting.Commands.Categorias.UpdateCategoria;
using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Accounting.Queries.Categorias.GetCategoriaById;
using BudgetCouple.Application.Accounting.Queries.Categorias.ListCategorias;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List all categorias.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> ListCategorias()
    {
        var query = new ListCategoriasQuery();
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Get a categoria by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(Guid id)
    {
        var query = new GetCategoriaByIdQuery(id);
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    /// <summary>
    /// Create a new categoria.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> CreateCategoria([FromBody] CreateCategoriaRequest request)
    {
        var command = new CreateCategoriaCommand(
            request.Nome,
            request.TipoCategoria,
            request.CorHex,
            request.Icone,
            request.ParentCategoriaId);

        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCategoria), new { id = result.Value.Id }, result.Value)
            : ToActionResult(result);
    }

    /// <summary>
    /// Update a categoria.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoriaDto>> UpdateCategoria(Guid id, [FromBody] UpdateCategoriaRequest request)
    {
        var command = new UpdateCategoriaCommand(id, request.Nome, request.CorHex, request.Icone);
        var result = await _mediator.Send(command);
        return ToActionResult(result);
    }

    /// <summary>
    /// Delete a categoria.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoria(Guid id)
    {
        var command = new DeleteCategoriaCommand(id);
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

public record CreateCategoriaRequest(
    string Nome,
    string TipoCategoria,
    string CorHex,
    string? Icone,
    Guid? ParentCategoriaId);

public record UpdateCategoriaRequest(
    string Nome,
    string CorHex,
    string? Icone);
