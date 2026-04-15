namespace BudgetCouple.Application.Accounting.Queries.Lancamentos.List;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class ListLancamentosQueryHandler : IRequestHandler<ListLancamentosQuery, Result<ListaLancamentosResponse>>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IContaRepository _contaRepository;
    private readonly ICartaoRepository _cartaoRepository;

    public ListLancamentosQueryHandler(
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IContaRepository contaRepository,
        ICartaoRepository cartaoRepository)
    {
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _contaRepository = contaRepository;
        _cartaoRepository = cartaoRepository;
    }

    public async Task<Result<ListaLancamentosResponse>> Handle(ListLancamentosQuery request, CancellationToken cancellationToken)
    {
        // Get lancamentos
        var lancamentos = await _lancamentoRepository.ListAsync(
            request.DataInicio,
            request.DataFim,
            request.ContaId,
            request.CartaoId,
            request.CategoriaId,
            request.Status,
            request.Tipo,
            request.NaturezaLancamento,
            request.Skip,
            request.Take,
            cancellationToken);

        // Get total count
        var total = await _lancamentoRepository.CountAsync(
            request.DataInicio,
            request.DataFim,
            request.ContaId,
            request.CartaoId,
            request.CategoriaId,
            request.Status,
            request.Tipo,
            request.NaturezaLancamento,
            cancellationToken);

        // Map to DTOs
        var dtos = new List<LancamentoDto>();
        var categoriaCache = new Dictionary<Guid, string>();
        var contaCache = new Dictionary<Guid, string>();
        var cartaoCache = new Dictionary<Guid, string>();

        foreach (var lancamento in lancamentos)
        {
            // Get categoria name
            string categoriaNome = "";
            if (!categoriaCache.TryGetValue(lancamento.CategoriaId, out var cachedCategoria))
            {
                var categoria = await _categoriaRepository.GetByIdAsync(lancamento.CategoriaId, cancellationToken);
                categoriaNome = categoria?.Nome ?? "";
                categoriaCache[lancamento.CategoriaId] = categoriaNome;
            }
            else
            {
                categoriaNome = cachedCategoria;
            }

            // Get conta name if needed
            string? contaNome = null;
            if (lancamento.ContaId.HasValue && !contaCache.TryGetValue(lancamento.ContaId.Value, out var cachedConta))
            {
                var conta = await _contaRepository.GetByIdAsync(lancamento.ContaId.Value, cancellationToken);
                contaNome = conta?.Nome;
                if (contaNome != null)
                    contaCache[lancamento.ContaId.Value] = contaNome;
            }
            else if (lancamento.ContaId.HasValue)
            {
                contaNome = contaCache[lancamento.ContaId.Value];
            }

            // Get cartao name if needed
            string? cartaoNome = null;
            if (lancamento.CartaoId.HasValue && !cartaoCache.TryGetValue(lancamento.CartaoId.Value, out var cachedCartao))
            {
                var cartao = await _cartaoRepository.GetByIdAsync(lancamento.CartaoId.Value, cancellationToken);
                cartaoNome = cartao?.Nome;
                if (cartaoNome != null)
                    cartaoCache[lancamento.CartaoId.Value] = cartaoNome;
            }
            else if (lancamento.CartaoId.HasValue)
            {
                cartaoNome = cartaoCache[lancamento.CartaoId.Value];
            }

            var dto = LancamentoDto.FromDomain(lancamento, contaNome, cartaoNome, categoriaNome);
            dtos.Add(dto);
        }

        return Result.Success(new ListaLancamentosResponse(dtos, total, request.Skip, request.Take));
    }
}
