namespace BudgetCouple.Application.Dashboard.Queries.GetDashboard;

using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Application.Dashboard.DTOs;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<DashboardDto>>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IContaRepository _contaRepository;
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IMetaRepository _metaRepository;
    private readonly ILogger<GetDashboardQueryHandler> _logger;

    public GetDashboardQueryHandler(
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IContaRepository contaRepository,
        ICartaoRepository cartaoRepository,
        IMetaRepository metaRepository,
        ILogger<GetDashboardQueryHandler> logger)
    {
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _contaRepository = contaRepository;
        _cartaoRepository = cartaoRepository;
        _metaRepository = metaRepository;
        _logger = logger;
    }

    public async Task<Result<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        // Parse competencia (YYYY-MM)
        if (!DateTime.TryParse($"{request.Mes}-01", out var dataBase))
            return Result.Failure<DashboardDto>(Error.Validation("Formato de mês inválido. Use YYYY-MM"));

        var primeiroDiaMes = new DateOnly(dataBase.Year, dataBase.Month, 1);
        var ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);

        // Fetch all lancamentos for the month
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

        // Get all contas and cartoes
        var contas = await _contaRepository.ListAsync(cancellationToken);
        var cartoes = await _cartaoRepository.ListAsync(cancellationToken);
        var categorias = await _categoriaRepository.ListAsync(cancellationToken);

        // Build caches
        var contaCache = contas.ToDictionary(c => c.Id);
        var cartaoCache = cartoes.ToDictionary(c => c.Id);
        var categoriaCache = categorias.ToDictionary(c => c.Id);

        // Calculate Resumo
        var lancamentosRealizados = lancamentos.Where(l => l.Natureza == NaturezaLancamento.REALIZADA).ToList();
        var lancamentosPrevistos = lancamentos.Where(l => l.Natureza == NaturezaLancamento.PREVISTA).ToList();

        var totalReceitasRealizadas = lancamentosRealizados
            .Where(l => l.Tipo == TipoLancamento.RECEITA)
            .Sum(l => l.Valor);

        var totalDespesasRealizadas = lancamentosRealizados
            .Where(l => l.Tipo == TipoLancamento.DESPESA)
            .Sum(l => l.Valor);

        var totalReceitasPrevistas = lancamentosPrevistos
            .Where(l => l.Tipo == TipoLancamento.RECEITA)
            .Sum(l => l.Valor);

        var totalDespesasPrevistas = lancamentosPrevistos
            .Where(l => l.Tipo == TipoLancamento.DESPESA)
            .Sum(l => l.Valor);

        var saldo = totalReceitasRealizadas - totalDespesasRealizadas;

        // Calculate saldo consolidado for active accounts
        var saldoConsolidadoContas = 0m;
        foreach (var conta in contas.Where(c => c.Ativa))
        {
            var entradas = lancamentosRealizados
                .Where(l => l.ContaId == conta.Id && l.Tipo == TipoLancamento.RECEITA)
                .Sum(l => l.Valor);
            var saidas = lancamentosRealizados
                .Where(l => l.ContaId == conta.Id && l.Tipo == TipoLancamento.DESPESA)
                .Sum(l => l.Valor);

            saldoConsolidadoContas += conta.SaldoInicial + entradas - saidas;
        }

        var resumo = new ResumoDto(
            totalReceitasRealizadas,
            totalDespesasRealizadas,
            saldo,
            totalReceitasPrevistas,
            totalDespesasPrevistas,
            saldoConsolidadoContas);

        // Calculate PorCategoria (TOP despesas)
        var porCategoria = lancamentosRealizados
            .GroupBy(l => l.CategoriaId)
            .Select(g =>
            {
                var categoria = categoriaCache.GetValueOrDefault(g.Key);
                var totalDespesas = g.Where(l => l.Tipo == TipoLancamento.DESPESA).Sum(l => l.Valor);
                var totalReceitas = g.Where(l => l.Tipo == TipoLancamento.RECEITA).Sum(l => l.Valor);
                var percentual = totalDespesasRealizadas > 0 ? (totalDespesas / totalDespesasRealizadas) * 100 : 0;

                return new CategoriaResumoDto(
                    g.Key,
                    categoria?.Nome ?? "Desconhecida",
                    categoria?.Cor ?? "#CCCCCC",
                    totalDespesas,
                    totalReceitas,
                    percentual);
            })
            .OrderByDescending(x => x.TotalDespesas)
            .ToList();

        // Calculate PorConta
        var porConta = contas
            .Where(c => c.Ativa)
            .Select(c =>
            {
                var lancamentosConta = lancamentosRealizados.Where(l => l.ContaId == c.Id).ToList();
                var totalEntradas = lancamentosConta.Where(l => l.Tipo == TipoLancamento.RECEITA).Sum(l => l.Valor);
                var totalSaidas = lancamentosConta.Where(l => l.Tipo == TipoLancamento.DESPESA).Sum(l => l.Valor);
                var saldoConta = c.SaldoInicial + totalEntradas - totalSaidas;

                return new ContaResumoDto(
                    c.Id,
                    c.Nome,
                    saldoConta,
                    totalEntradas,
                    totalSaidas);
            })
            .ToList();

        // Calculate PorCartao
        var porCartao = cartoes
            .Where(c => c.Ativa)
            .Select(c =>
            {
                var lancamentosCartao = lancamentosRealizados.Where(l => l.CartaoId == c.Id).ToList();
                var valorFatura = lancamentosCartao.Where(l => l.Tipo == TipoLancamento.DESPESA).Sum(l => l.Valor);
                var limiteUtilizadoPct = c.Limite > 0 ? (valorFatura / c.Limite) * 100 : 0;

                return new CartaoResumoDto(
                    c.Id,
                    c.Nome,
                    valorFatura,
                    c.Limite,
                    limiteUtilizadoPct);
            })
            .ToList();

        // Calculate EvolucaoMensal (últimos 6 meses)
        var evolucaoMensal = new List<EvolucaoMesDto>();
        for (int i = 5; i >= 0; i--)
        {
            var mesData = primeiroDiaMes.AddMonths(-i);
            var inicio = new DateOnly(mesData.Year, mesData.Month, 1);
            var fim = inicio.AddMonths(1).AddDays(-1);

            var lancamentosMes = lancamentos
                .Where(l => l.Data >= inicio && l.Data <= fim && l.Natureza == NaturezaLancamento.REALIZADA)
                .ToList();

            var receitas = lancamentosMes.Where(l => l.Tipo == TipoLancamento.RECEITA).Sum(l => l.Valor);
            var despesas = lancamentosMes.Where(l => l.Tipo == TipoLancamento.DESPESA).Sum(l => l.Valor);

            evolucaoMensal.Add(new EvolucaoMesDto(
                $"{mesData.Year:0000}-{mesData.Month:00}",
                receitas,
                despesas,
                receitas - despesas));
        }

        // Calculate ProximosVencimentos (próximos 7 dias)
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var proximosVencimentos = lancamentos
            .Where(l => l.Natureza == NaturezaLancamento.PREVISTA &&
                       l.Data >= hoje &&
                       l.Data <= hoje.AddDays(7))
            .Select(l => new VencimentoProximoDto(
                l.Id,
                l.Descricao ?? "Lançamento",
                l.Valor,
                l.Data))
            .OrderBy(v => v.DataVencimento)
            .ToList();

        // Get budget alerts for current month (inline to avoid nested transactions)
        var alertasOrcamento = await BuildAlertasOrcamento(lancamentos, cancellationToken);

        var dashboard = new DashboardDto(
            request.Mes,
            resumo,
            porCategoria,
            porConta,
            porCartao,
            evolucaoMensal,
            alertasOrcamento,
            proximosVencimentos);

        sw.Stop();
        _logger.LogInformation("Dashboard gerado em {ElapsedMs}ms", sw.ElapsedMilliseconds);

        return Result.Success(dashboard);
    }

    private async Task<List<AlertaOrcamentoDto>> BuildAlertasOrcamento(List<Lancamento> lancamentos, CancellationToken cancellationToken)
    {
        try
        {
            var alertas = new List<AlertaOrcamentoDto>();

            // Get all REDUCAO_CATEGORIA metas
            var metas = await _metaRepository.ListAsync(cancellationToken);
            var metasReducao = metas.OfType<MetaReducaoCategoria>().ToList();

            // Get current month date range
            var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
            var primeiroDiaMes = new DateOnly(hoje.Year, hoje.Month, 1);
            var ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);

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

                    alertas.Add(new AlertaOrcamentoDto(
                        categoriaNome ?? "Desconhecida",
                        gastoMes,
                        meta.ValorAlvo,
                        percentualUtilizado));
                }
            }

            return alertas;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao buscar alertas de orçamento para dashboard");
            return new List<AlertaOrcamentoDto>();
        }
    }
}
