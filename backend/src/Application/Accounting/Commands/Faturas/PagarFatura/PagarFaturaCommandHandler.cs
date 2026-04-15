using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Common;
using MediatR;

namespace BudgetCouple.Application.Accounting.Commands.Faturas.PagarFatura;

public class PagarFaturaCommandHandler : IRequestHandler<PagarFaturaCommand, Result<FaturaDto>>
{
    private readonly ICartaoRepository _cartaoRepository;
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IContaRepository _contaRepository;
    private readonly IMediator _mediator;

    public PagarFaturaCommandHandler(
        ICartaoRepository cartaoRepository,
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IContaRepository contaRepository,
        IMediator mediator)
    {
        _cartaoRepository = cartaoRepository;
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _contaRepository = contaRepository;
        _mediator = mediator;
    }

    public async Task<Result<FaturaDto>> Handle(PagarFaturaCommand request, CancellationToken cancellationToken)
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

        if (lancamentosCompetencia.Count == 0)
            return Result.Failure<FaturaDto>(Error.NotFound("Nenhum lançamento encontrado para esta competência."));

        // Check if already paid
        if (lancamentosCompetencia.All(l => l.FaturaPaga))
            return Result.Failure<FaturaDto>(Error.Conflict("Fatura já foi paga."));

        // Determine data pagamento and conta debito
        var dataPagamento = request.DataPagamento ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var contaDebito = request.ContaDebitoIdOverride ?? cartao.ContaPagamentoId;

        // Verify conta exists
        var conta = await _contaRepository.GetByIdAsync(contaDebito, cancellationToken);
        if (conta == null)
            return Result.Failure<FaturaDto>(Error.NotFound("Conta de débito não encontrada."));

        // Get the payment category
        var categoriaPagamento = await _categoriaRepository.GetByNomeAsync("Pagamento de Fatura de Cartão", cancellationToken);
        if (categoriaPagamento == null)
            return Result.Failure<FaturaDto>(Error.NotFound("Categoria 'Pagamento de Fatura de Cartão' não encontrada."));

        // Calculate total
        var valorTotal = lancamentosCompetencia.Sum(l => l.Valor);

        // Create payment transaction
        var lancamentoPagamento = Lancamento.CriarSimples(
            tipo: TipoLancamento.DESPESA,
            natureza: NaturezaLancamento.REALIZADA,
            valor: valorTotal,
            data: dataPagamento,
            categoriaId: categoriaPagamento.Id,
            contaId: contaDebito,
            descricao: $"Pagamento de Fatura - {cartao.Nome} {request.Competencia}",
            classificacao: ClassificacaoRecorrencia.VARIAVEL
        );

        if (lancamentoPagamento.IsFailure)
            return Result.Failure<FaturaDto>(lancamentoPagamento.Error);

        // Mark payment as paid immediately
        lancamentoPagamento.Value.Pagar(dataPagamento, contaDebito);

        // Marcar como pagamento de fatura para que dashboard não conte como despesa nova
        // (a despesa já foi reconhecida no mês de competência das compras originais).
        // O lançamento continua afetando saldo da conta (é débito real), mas não entra
        // no total de despesas nem nas agregações por categoria/evolução.
        if (!lancamentoPagamento.Value.Tags.Contains("__PAGAMENTO_FATURA__"))
            lancamentoPagamento.Value.Tags.Add("__PAGAMENTO_FATURA__");

        // Add payment transaction
        _lancamentoRepository.Add(lancamentoPagamento.Value);

        // Mark all invoice lancamentos as paid
        foreach (var lancamento in lancamentosCompetencia)
        {
            lancamento.MarcarFaturaPaga(DateTime.UtcNow);
            _lancamentoRepository.Update(lancamento);
        }

        // Update conta balance (deduct payment)
        conta.AtualizarSaldo(-valorTotal);
        _contaRepository.Update(conta);

        // Build and return updated fatura DTO
        var getQuery = new Queries.Faturas.GetFatura.GetFaturaQuery(request.CartaoId, request.Competencia);
        return await _mediator.Send(getQuery, cancellationToken);
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
