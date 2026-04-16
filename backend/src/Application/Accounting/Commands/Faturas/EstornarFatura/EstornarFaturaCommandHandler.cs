using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Faturas;
using BudgetCouple.Domain.Common;
using MediatR;

namespace BudgetCouple.Application.Accounting.Commands.Faturas.EstornarFatura;

public class EstornarFaturaCommandHandler : IRequestHandler<EstornarFaturaCommand, Result<FaturaDto>>
{
    private readonly ICartaoRepository _cartaoRepository;
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly IContaRepository _contaRepository;
    private readonly IApplicationDbContext _dbContext;

    public EstornarFaturaCommandHandler(
        ICartaoRepository cartaoRepository,
        ILancamentoRepository lancamentoRepository,
        IContaRepository contaRepository,
        IApplicationDbContext dbContext)
    {
        _cartaoRepository = cartaoRepository;
        _lancamentoRepository = lancamentoRepository;
        _contaRepository = contaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<FaturaDto>> Handle(EstornarFaturaCommand request, CancellationToken cancellationToken)
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
            take: 1000,
            cancellationToken: cancellationToken);

        // Filter lancamentos by competencia
        var lancamentosCompetencia = lancamentos
            .Where(l =>
            {
                var competencia = cartao.CalcularCompetenciaFatura(l.Data);
                return competencia.Year == ano && competencia.Month == mes;
            })
            .ToList();

        // Check if fatura is actually paid
        if (!lancamentosCompetencia.All(l => l.FaturaPaga))
            return Result.Failure<FaturaDto>(Error.Validation("Fatura não está paga. Não é possível estornar."));

        // Find the payment lancamento (by tag + description pattern)
        var descricaoPagamento = $"Pagamento de Fatura - {cartao.Nome} {request.Competencia}";
        var allLancamentos = await _lancamentoRepository.ListAsync(
            contaId: null,
            cartaoId: null,
            take: 5000,
            cancellationToken: cancellationToken);

        var lancamentoPagamento = allLancamentos
            .FirstOrDefault(l =>
                l.Tags.Contains("__PAGAMENTO_FATURA__") &&
                l.Descricao == descricaoPagamento);

        // Revert conta balance if payment lancamento found
        if (lancamentoPagamento != null)
        {
            // Restore the amount to the conta
            if (lancamentoPagamento.ContaId.HasValue)
            {
                var conta = await _contaRepository.GetByIdAsync(lancamentoPagamento.ContaId.Value, cancellationToken);
                if (conta != null)
                {
                    conta.AtualizarSaldo(lancamentoPagamento.Valor); // credit back
                    _contaRepository.Update(conta);
                }
            }

            // Delete the payment lancamento
            _lancamentoRepository.Delete(lancamentoPagamento);
        }

        // Unmark all invoice lancamentos as paid
        foreach (var lancamento in lancamentosCompetencia)
        {
            lancamento.DesmarcarFaturaPaga();
            _lancamentoRepository.Update(lancamento);
        }

        // Persist all changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Build fatura DTO
        var fatura = new Fatura(cartao, new DateOnly(ano, mes, 1), lancamentosCompetencia);

        var lancamentoDtos = lancamentosCompetencia.Select(l => LancamentoDto.FromDomain(l)).ToList();

        var dto = new FaturaDto(
            CartaoId: cartao.Id,
            CartaoNome: cartao.Nome,
            Competencia: request.Competencia,
            DataFechamento: fatura.DataFechamento.ToString("yyyy-MM-dd"),
            DataVencimento: fatura.DataVencimento.ToString("yyyy-MM-dd"),
            ValorTotal: fatura.Total,
            Paga: false,
            DataPagamento: null,
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
}
