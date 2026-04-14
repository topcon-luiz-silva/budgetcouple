using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

namespace BudgetCouple.Application.Accounting.Queries.Faturas.ListFaturas;

public class ListFaturasQueryHandler : IRequestHandler<ListFaturasQuery, Result<List<FaturaResumoDto>>>
{
    private readonly ICartaoRepository _cartaoRepository;
    private readonly ILancamentoRepository _lancamentoRepository;

    public ListFaturasQueryHandler(ICartaoRepository cartaoRepository, ILancamentoRepository lancamentoRepository)
    {
        _cartaoRepository = cartaoRepository;
        _lancamentoRepository = lancamentoRepository;
    }

    public async Task<Result<List<FaturaResumoDto>>> Handle(ListFaturasQuery request, CancellationToken cancellationToken)
    {
        // Get cartao
        var cartao = await _cartaoRepository.GetByIdAsync(request.CartaoId, cancellationToken);
        if (cartao == null)
            return Result.Failure<List<FaturaResumoDto>>(Error.NotFound("Cartão não encontrado."));

        // Get all lancamentos for this cartao
        var lancamentos = await _lancamentoRepository.ListAsync(
            cartaoId: request.CartaoId,
            take: 1000, // get all
            cancellationToken: cancellationToken);

        // Group by competencia (last 12 months)
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var competencias = new Dictionary<string, List<Domain.Accounting.Lancamentos.Lancamento>>();

        // Generate last 12 months competencias
        for (int i = 11; i >= 0; i--)
        {
            var dataBase = hoje.AddMonths(-i);
            var competenciaKey = dataBase.ToString("yyyy-MM");
            competencias[competenciaKey] = new List<Domain.Accounting.Lancamentos.Lancamento>();
        }

        // Group lancamentos by competencia
        foreach (var lancamento in lancamentos)
        {
            var competencia = cartao.CalcularCompetenciaFatura(lancamento.Data);
            var competenciaKey = competencia.ToString("yyyy-MM");

            if (competencias.ContainsKey(competenciaKey))
            {
                competencias[competenciaKey].Add(lancamento);
            }
        }

        // Build DTOs (reverse order: most recent first)
        var dtos = competencias
            .OrderByDescending(kvp => kvp.Key)
            .Select(kvp =>
            {
                var competencia = kvp.Key;
                var lancamentosComp = kvp.Value;

                // Parse competencia
                var parts = competencia.Split('-');
                var ano = int.Parse(parts[0]);
                var mes = int.Parse(parts[1]);

                // Calculate due date
                var competenciaDate = new DateOnly(ano, mes, 1);
                var mesVencimento = competenciaDate.AddMonths(1);
                int dia = Math.Min(cartao.DiaVencimento, DateTime.DaysInMonth(mesVencimento.Year, mesVencimento.Month));
                var dataVencimento = new DateOnly(mesVencimento.Year, mesVencimento.Month, dia);

                // Check if all are paid
                var todosPagos = lancamentosComp.Count > 0 && lancamentosComp.All(l => l.FaturaPaga);
                var valorTotal = lancamentosComp.Sum(l => l.Valor);

                return new FaturaResumoDto(
                    CartaoId: cartao.Id,
                    Competencia: competencia,
                    ValorTotal: valorTotal,
                    Paga: todosPagos,
                    DataVencimento: dataVencimento.ToString("yyyy-MM-dd")
                );
            })
            .ToList();

        return Result.Success(dtos);
    }
}
