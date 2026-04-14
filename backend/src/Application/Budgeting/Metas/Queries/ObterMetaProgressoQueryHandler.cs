namespace BudgetCouple.Application.Budgeting.Metas.Queries;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// Calculates the current progress of a Meta by:
/// - For ECONOMIA: Summing RECEITA lancamentos with data <= hoje and data >= data de criação
/// - For REDUCAO_CATEGORIA: Summing DESPESA lancamentos for this month in the specific categoria
/// </summary>
public class ObterMetaProgressoQueryHandler : IRequestHandler<ObterMetaProgressoQuery, Result<MetaProgressoDto>>
{
    private readonly IMetaRepository _metaRepository;
    private readonly ILancamentoRepository _lancamentoRepository;

    public ObterMetaProgressoQueryHandler(IMetaRepository metaRepository, ILancamentoRepository lancamentoRepository)
    {
        _metaRepository = metaRepository;
        _lancamentoRepository = lancamentoRepository;
    }

    public async Task<Result<MetaProgressoDto>> Handle(ObterMetaProgressoQuery request, CancellationToken cancellationToken)
    {
        var meta = await _metaRepository.GetByIdAsync(request.MetaId, cancellationToken);
        if (meta == null)
            return Result.Failure<MetaProgressoDto>(Error.NotFound("Meta não encontrada."));

        decimal valorAtualCalculado = 0;
        int? diasRestantes = null;

        if (meta is MetaEconomia metaEconomia)
        {
            // Sum RECEITA lancamentos up to today
            var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
            var lancamentos = await _lancamentoRepository.ListAsync(
                new DateOnly(meta.CriadoEm.Year, meta.CriadoEm.Month, meta.CriadoEm.Day),
                hoje,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                int.MaxValue,
                cancellationToken);

            valorAtualCalculado = lancamentos
                .Where(l => l.Tipo == TipoLancamento.RECEITA && l.Natureza == NaturezaLancamento.REALIZADA)
                .Sum(l => l.Valor);

            // Calculate remaining days
            if (meta.DataLimite.HasValue)
            {
                var diasRestantesCalc = (meta.DataLimite.Value.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).Days;
                diasRestantes = Math.Max(0, diasRestantesCalc);
            }
        }
        else if (meta is MetaReducaoCategoria metaReducao)
        {
            // Sum DESPESA lancamentos for current month in this categoria
            var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
            var primeiroDiaMes = new DateOnly(hoje.Year, hoje.Month, 1);
            var ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);

            var lancamentos = await _lancamentoRepository.ListAsync(
                primeiroDiaMes,
                ultimoDiaMes,
                null,
                null,
                metaReducao.CategoriaId,
                null,
                null,
                null,
                0,
                int.MaxValue,
                cancellationToken);

            valorAtualCalculado = lancamentos
                .Where(l => l.Tipo == TipoLancamento.DESPESA && l.Natureza == NaturezaLancamento.REALIZADA)
                .Sum(l => l.Valor);
        }

        // Update meta's ValorAtual
        meta.AtualizarValorAtual(valorAtualCalculado);
        await _metaRepository.SaveChangesAsync(cancellationToken);

        var percentual = meta.PercentualProgresso();
        var atingiuAlerta = meta.AtingiuAlerta();
        var atingida = meta.Atingida();

        var dto = new MetaProgressoDto(
            meta.Id,
            meta.Tipo,
            valorAtualCalculado,
            meta.ValorAlvo,
            percentual,
            diasRestantes,
            atingiuAlerta,
            atingida);

        return Result.Success(dto);
    }
}
