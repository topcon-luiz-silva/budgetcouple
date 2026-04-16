using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Faturas;
using BudgetCouple.Domain.Common;
using MediatR;

namespace BudgetCouple.Application.Accounting.Queries.Faturas.GetFatura;

public class GetFaturaQueryHandler : IRequestHandler<GetFaturaQuery, Result<FaturaDto>>
{
    private readonly ICartaoRepository _cartaoRepository;
    private readonly ILancamentoRepository _lancamentoRepository;

    public GetFaturaQueryHandler(ICartaoRepository cartaoRepository, ILancamentoRepository lancamentoRepository)
    {
        _cartaoRepository = cartaoRepository;
        _lancamentoRepository = lancamentoRepository;
    }

    public async Task<Result<FaturaDto>> Handle(GetFaturaQuery request, CancellationToken cancellationToken)
    {
        // Parse competencia YYYY-MM
        if (!TryParseCompetencia(request.Competencia, out var ano, out var mes))
            return Result.Failure<FaturaDto>(Error.Validation("Competência inválida. Use formato YYYY-MM."));

        // Get cartao
        var cartao = await _cartaoRepository.GetByIdAsync(request.CartaoId, cancellationToken);
        if (cartao == null)
            return Result.Failure<FaturaDto>(Error.NotFound("Cartão não encontrado."));

        // Get lancamentos for this cartao
        var lancamentos = await _lancamentoRepository.ListAsync(
            cartaoId: request.CartaoId,
            take: 1000, // get all
            cancellationToken: cancellationToken);

        // Filter lancamentos by competencia
        var lancamentosCompetencia = lancamentos
            .Where(l =>
            {
                var competencia = cartao.CalcularCompetenciaFatura(l.Data);
                return competencia.Year == ano && competencia.Month == mes;
            })
            .ToList();

        // Build fatura
        var fatura = new Fatura(cartao, new DateOnly(ano, mes, 1), lancamentosCompetencia);

        // Map to DTO
        var lancamentoDtos = lancamentosCompetencia.Select(l => MapLancamentoToDto(l)).ToList();

        // Check if all lancamentos are paid (fatura paga)
        var todosPagos = lancamentosCompetencia.Count > 0 && lancamentosCompetencia.All(l => l.FaturaPaga);
        var dataPagamento = todosPagos && lancamentosCompetencia.Count > 0
            ? lancamentosCompetencia.First(l => l.FaturaPaga).FaturaPagaEm?.ToString("yyyy-MM-dd")
            : null;

        var dto = new FaturaDto(
            CartaoId: cartao.Id,
            CartaoNome: cartao.Nome,
            Competencia: request.Competencia,
            DataFechamento: fatura.DataFechamento.ToString("yyyy-MM-dd"),
            DataVencimento: fatura.DataVencimento.ToString("yyyy-MM-dd"),
            ValorTotal: fatura.Total,
            Paga: todosPagos,
            DataPagamento: dataPagamento,
            Lancamentos: lancamentoDtos
        );

        return Result.Success(dto);
    }

    private static bool TryParseCompetencia(string competencia, out int ano, out int mes)
    {
        ano = 0;
        mes = 0;

        if (string.IsNullOrWhiteSpace(competencia))
            return false;

        var parts = competencia.Split('-');
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], out ano) || !int.TryParse(parts[1], out mes))
            return false;

        if (mes < 1 || mes > 12)
            return false;

        return true;
    }

    private static LancamentoDto MapLancamentoToDto(Domain.Accounting.Lancamentos.Lancamento l) =>
        LancamentoDto.FromDomain(l);

    private static LancamentoDto MapLancamentoToDto_UNUSED(Domain.Accounting.Lancamentos.Lancamento l) =>
        new LancamentoDto(
            Id: l.Id,
            Descricao: l.Descricao ?? "",
            Valor: l.Valor,
            DataCompetencia: l.Data,
            DataVencimento: null,
            DataPagamento: l.DataPagamento,
            TipoLancamento: l.Tipo.ToString(),
            NaturezaLancamento: l.Tipo.ToString(),
            StatusPagamento: l.StatusPagamento.ToString(),
            ContaId: l.ContaId,
            ContaNome: null,
            CartaoId: l.CartaoId,
            CartaoNome: null,
            CategoriaId: l.CategoriaId,
            CategoriaNome: "",
            SubcategoriaId: l.SubcategoriaId,
            SubcategoriaNome: null,
            Parcela: null,
            TotalParcelas: null,
            RecorrenciaId: l.RecorrenciaId,
            LancamentoPaiId: null,
            Tags: l.Tags,
            Observacoes: null,
            CriadoEm: l.CriadoEm,
            AtualizadoEm: l.AtualizadoEm
        );
}
