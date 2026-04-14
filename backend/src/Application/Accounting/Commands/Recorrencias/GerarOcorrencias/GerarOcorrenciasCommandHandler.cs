namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.GerarOcorrencias;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Common;
using MediatR;
using System.Text.Json;

public class GerarOcorrenciasCommandHandler : IRequestHandler<GerarOcorrenciasCommand, Result<List<LancamentoDto>>>
{
    private readonly IRecorrenciaRepository _recorrenciaRepository;
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _dbContext;

    public GerarOcorrenciasCommandHandler(
        IRecorrenciaRepository recorrenciaRepository,
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IApplicationDbContext dbContext)
    {
        _recorrenciaRepository = recorrenciaRepository;
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<List<LancamentoDto>>> Handle(GerarOcorrenciasCommand request, CancellationToken cancellationToken)
    {
        var recorrencia = await _recorrenciaRepository.GetByIdAsync(request.RecorrenciaId, cancellationToken);
        if (recorrencia == null)
            return Result.Failure<List<LancamentoDto>>(Error.NotFound($"Recorrência com ID {request.RecorrenciaId} não encontrada"));

        // Get existing lancamentos for this recorrencia
        var existentes = await _lancamentoRepository.GetByRecorrenciaIdAsync(request.RecorrenciaId, cancellationToken);
        var datesExistentes = existentes.Select(l => l.Data).ToHashSet();

        // Generate new snapshots
        var snapshots = recorrencia.GerarProximasOcorrencias(request.Ate);

        // Filter out existing dates
        var novasSnapshots = snapshots.Where(s => !datesExistentes.Contains(s.DataOcorrencia)).ToList();

        if (novasSnapshots.Count == 0)
            return Result.Success(new List<LancamentoDto>());

        // Parse template to get base data
        var template = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(recorrencia.TemplateJson);
        if (template == null)
            return Result.Failure<List<LancamentoDto>>(Error.Validation("Template JSON inválido"));

        var categoriaId = Guid.Parse(template["categoriaId"].GetString() ?? "");
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId, cancellationToken);
        var categoriaNome = categoria?.Nome ?? "";

        var lancamentos = new List<Lancamento>();
        foreach (var snapshot in novasSnapshots)
        {
            var valor = template["valor"].GetDecimal();
            var naturezaStr = template["natureza"].GetString() ?? "PREVISTA";
            var descricao = template["descricaoBase"].GetString() ?? "";

            if (!Enum.TryParse<NaturezaLancamento>(naturezaStr, out var natureza))
                continue;

            var contaId = template.ContainsKey("contaId") && template["contaId"].ValueKind != System.Text.Json.JsonValueKind.Null
                ? Guid.Parse(template["contaId"].GetString() ?? "")
                : (Guid?)null;

            var cartaoId = template.ContainsKey("cartaoId") && template["cartaoId"].ValueKind != System.Text.Json.JsonValueKind.Null
                ? Guid.Parse(template["cartaoId"].GetString() ?? "")
                : (Guid?)null;

            var tipo = naturezaStr == "RECEITA" ? TipoLancamento.RECEITA : TipoLancamento.DESPESA;

            var lancResult = Lancamento.CriarRecorrenciaOcorrencia(
                tipo,
                natureza,
                valor,
                snapshot.DataOcorrencia,
                categoriaId,
                recorrencia.Id,
                null,
                contaId,
                cartaoId,
                descricao);

            if (lancResult.IsSuccess)
                lancamentos.Add(lancResult.Value);
        }

        _lancamentoRepository.AddRange(lancamentos);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dtos = lancamentos.Select(l => LancamentoDto.FromDomain(l, categoriaNome: categoriaNome)).ToList();
        return Result.Success(dtos);
    }
}
