namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateSimples;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Common;
using MediatR;

public class CreateLancamentoSimplesCommandHandler : IRequestHandler<CreateLancamentoSimplesCommand, Result<LancamentoDto>>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IContaRepository _contaRepository;
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IApplicationDbContext _dbContext;

    public CreateLancamentoSimplesCommandHandler(
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IContaRepository contaRepository,
        ICartaoRepository cartaoRepository,
        IApplicationDbContext dbContext)
    {
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _contaRepository = contaRepository;
        _cartaoRepository = cartaoRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<LancamentoDto>> Handle(CreateLancamentoSimplesCommand request, CancellationToken cancellationToken)
    {
        // Validate categoria exists
        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);
        if (categoria == null)
            return Result.Failure<LancamentoDto>(Error.NotFound($"Categoria com ID {request.CategoriaId} não encontrada"));

        // Validate conta exists if provided
        string? contaNome = null;
        if (request.ContaId.HasValue)
        {
            var conta = await _contaRepository.GetByIdAsync(request.ContaId.Value, cancellationToken);
            if (conta == null)
                return Result.Failure<LancamentoDto>(Error.NotFound($"Conta com ID {request.ContaId} não encontrada"));
            contaNome = conta.Nome;
        }

        // Validate cartao exists if provided
        string? cartaoNome = null;
        if (request.CartaoId.HasValue)
        {
            var cartao = await _cartaoRepository.GetByIdAsync(request.CartaoId.Value, cancellationToken);
            if (cartao == null)
                return Result.Failure<LancamentoDto>(Error.NotFound($"Cartão com ID {request.CartaoId} não encontrada"));
            cartaoNome = cartao.Nome;
        }

        // Map request.NaturezaLancamento (RECEITA|DESPESA|TRANSFERENCIA) → TipoLancamento
        var tipo = request.NaturezaLancamento == "RECEITA"
            ? TipoLancamento.RECEITA
            : TipoLancamento.DESPESA;

        // Map request.StatusPagamento (PREVISTO|REALIZADO|ATRASADO) → NaturezaLancamento (PREVISTA|REALIZADA)
        var natureza = request.StatusPagamento == "REALIZADO"
            ? NaturezaLancamento.REALIZADA
            : NaturezaLancamento.PREVISTA;

        var result = Lancamento.CriarSimples(
            tipo,
            natureza,
            request.Valor,
            request.DataCompetencia,
            request.CategoriaId,
            request.SubcategoriaId,
            request.ContaId,
            request.CartaoId,
            request.Descricao);

        if (!result.IsSuccess)
            return Result.Failure<LancamentoDto>(result.Error);

        var lancamento = result.Value;

        // Set tags if provided
        if (request.Tags != null && request.Tags.Count > 0)
        {
            lancamento.Tags.AddRange(request.Tags);
        }

        // If REALIZADO, mark as paid
        if (request.StatusPagamento == "REALIZADO")
        {
            var pagtoResult = lancamento.Pagar(request.DataCompetencia, request.ContaId ?? request.CartaoId);
            if (!pagtoResult.IsSuccess)
                return Result.Failure<LancamentoDto>(pagtoResult.Error);
        }

        _lancamentoRepository.Add(lancamento);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = LancamentoDto.FromDomain(lancamento, contaNome, cartaoNome, categoria.Nome);
        return Result.Success(dto);
    }
}
