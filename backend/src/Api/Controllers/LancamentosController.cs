namespace BudgetCouple.Api.Controllers;

using BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateParcelado;
using BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateRecorrencia;
using BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateSimples;
using BudgetCouple.Application.Accounting.Commands.Lancamentos.Delete;
using BudgetCouple.Application.Accounting.Commands.Lancamentos.Pagar;
using BudgetCouple.Application.Accounting.Commands.Lancamentos.Update;
using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Accounting.Queries.Lancamentos.GetById;
using BudgetCouple.Application.Accounting.Queries.Lancamentos.List;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LancamentosController : ControllerBase
{
    private readonly IMediator _mediator;

    public LancamentosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<(List<LancamentoDto>, int)>> List(
        [FromQuery] DateOnly? dataInicio,
        [FromQuery] DateOnly? dataFim,
        [FromQuery] Guid? contaId,
        [FromQuery] Guid? cartaoId,
        [FromQuery] Guid? categoriaId,
        [FromQuery] string? status,
        [FromQuery] string? tipo,
        [FromQuery] string? naturezaLancamento,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var query = new ListLancamentosQuery(
            dataInicio, dataFim, contaId, cartaoId, categoriaId, status, tipo, naturezaLancamento, skip, take);
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LancamentoDto>> GetById(Guid id)
    {
        var query = new GetLancamentoByIdQuery(id);
        var result = await _mediator.Send(query);
        return ToActionResult(result);
    }

    [HttpPost("simples")]
    public async Task<ActionResult<LancamentoDto>> CreateSimples([FromBody] CreateLancamentoSimplesRequest request)
    {
        var command = new CreateLancamentoSimplesCommand(
            request.Descricao,
            request.Valor,
            request.DataCompetencia,
            request.DataVencimento,
            request.NaturezaLancamento,
            request.ContaId,
            request.CartaoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.Tags,
            request.Observacoes,
            request.StatusPagamento);

        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : ToActionResult(result);
    }

    [HttpPost("parcelado")]
    public async Task<ActionResult<List<LancamentoDto>>> CreateParcelado([FromBody] CreateLancamentoParceladoRequest request)
    {
        var command = new CreateLancamentoParceladoCommand(
            request.DescricaoBase,
            request.ValorTotal,
            request.TotalParcelas,
            request.DataPrimeiraParcela,
            request.NaturezaLancamento,
            request.ContaId,
            request.CartaoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.Tags,
            request.Observacoes);

        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result.Value)
            : ToActionResult(result);
    }

    [HttpPost("recorrencia")]
    public async Task<ActionResult<(RecorrenciaDto, List<LancamentoDto>)>> CreateRecorrencia([FromBody] CreateRecorrenciaRequest request)
    {
        var command = new CreateRecorrenciaCommand(
            request.DescricaoBase,
            request.ValorBase,
            request.Frequencia,
            request.DataInicio,
            request.DataFim,
            request.NaturezaLancamento,
            request.ContaId,
            request.CartaoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.Tags,
            request.Observacoes,
            request.GerarOcorrenciasAte);

        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(result.Value)
            : ToActionResult(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LancamentoDto>> Update(Guid id, [FromBody] UpdateLancamentoRequest request)
    {
        var command = new UpdateLancamentoCommand(
            id,
            request.Descricao,
            request.Valor,
            request.DataCompetencia,
            request.CategoriaId,
            request.SubcategoriaId,
            request.Tags);

        var result = await _mediator.Send(command);
        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] string? escopo)
    {
        var command = new DeleteLancamentoCommand(id, escopo);
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? NoContent()
            : StatusCode(MapErrorToStatusCode(result.Error), new { error = result.Error.Message });
    }

    [HttpPost("{id}/pagar")]
    public async Task<ActionResult<LancamentoDto>> Pagar(Guid id, [FromBody] PagarLancamentoRequest request)
    {
        var command = new PagarLancamentoCommand(id, request.DataPagamento, request.ContaDebitoId);
        var result = await _mediator.Send(command);
        return ToActionResult(result);
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

public record CreateLancamentoSimplesRequest(
    string Descricao,
    decimal Valor,
    DateOnly DataCompetencia,
    DateOnly? DataVencimento,
    string NaturezaLancamento,
    Guid? ContaId,
    Guid? CartaoId,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    List<string>? Tags,
    string? Observacoes,
    string StatusPagamento);

public record CreateLancamentoParceladoRequest(
    string DescricaoBase,
    decimal ValorTotal,
    int TotalParcelas,
    DateOnly DataPrimeiraParcela,
    string NaturezaLancamento,
    Guid? ContaId,
    Guid? CartaoId,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    List<string>? Tags,
    string? Observacoes);

public record CreateRecorrenciaRequest(
    string DescricaoBase,
    decimal ValorBase,
    string Frequencia,
    DateOnly DataInicio,
    DateOnly? DataFim,
    string NaturezaLancamento,
    Guid? ContaId,
    Guid? CartaoId,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    List<string>? Tags,
    string? Observacoes,
    DateOnly? GerarOcorrenciasAte);

public record UpdateLancamentoRequest(
    string Descricao,
    decimal Valor,
    DateOnly DataCompetencia,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    List<string>? Tags);

public record PagarLancamentoRequest(
    DateOnly DataPagamento,
    Guid? ContaDebitoId);
