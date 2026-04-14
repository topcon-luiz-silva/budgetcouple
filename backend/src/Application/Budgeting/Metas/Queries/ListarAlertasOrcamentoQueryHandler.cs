namespace BudgetCouple.Application.Budgeting.Metas.Queries;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Common;
using MediatR;

public class ListarAlertasOrcamentoQueryHandler : IRequestHandler<ListarAlertasOrcamentoQuery, Result<List<AlertaOrcamentoDashboardDto>>>
{
    private readonly IMetaRepository _metaRepository;
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public ListarAlertasOrcamentoQueryHandler(
        IMetaRepository metaRepository,
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository)
    {
        _metaRepository = metaRepository;
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Result<List<AlertaOrcamentoDashboardDto>>> Handle(
        ListarAlertasOrcamentoQuery request,
        CancellationToken cancellationToken)
    {
        var alertas = new List<AlertaOrcamentoDashboardDto>();

        // Get all REDUCAO_CATEGORIA metas
        var metas = await _metaRepository.ListAsync(cancellationToken);
        var metasReducao = metas.OfType<MetaReducaoCategoria>().ToList();

        // Get current month date range
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var primeiroDiaMes = new DateOnly(hoje.Year, hoje.Month, 1);
        var ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);

        // Get all lancamentos for this month
        var lancamentos = await _lancamentoRepository.ListAsync(
            primeiroDiaMes,
            ultimoDiaMes,
            null,
            null,
            null,
            null,
            null,
            null,
            0,
            int.MaxValue,
            cancellationToken);

        // Get all categorias
        var categorias = await _categoriaRepository.ListAsync(cancellationToken);
        var categoriaCache = categorias.ToDictionary(c => c.Id);

        // Calculate alerts for each REDUCAO_CATEGORIA meta
        foreach (var meta in metasReducao)
        {
            if (!meta.CategoriaId.HasValue || !meta.Ativa)
                continue;

            var gastoMes = lancamentos
                .Where(l => l.CategoriaId == meta.CategoriaId &&
                           l.Tipo == TipoLancamento.DESPESA &&
                           l.Natureza == NaturezaLancamento.REALIZADA)
                .Sum(l => l.Valor);

            var percentualUtilizado = meta.ValorAlvo > 0 ? (gastoMes / meta.ValorAlvo) * 100 : 0;

            // Create alert if spending >= alert threshold
            if (percentualUtilizado >= meta.PercentualAlerta)
            {
                var categoriaNome = categoriaCache.GetValueOrDefault(meta.CategoriaId.Value)?.Nome;

                alertas.Add(new AlertaOrcamentoDashboardDto(
                    meta.Id,
                    meta.Nome,
                    categoriaNome,
                    gastoMes,
                    meta.ValorAlvo,
                    percentualUtilizado));
            }
        }

        return Result.Success(alertas);
    }
}
