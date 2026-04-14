namespace BudgetCouple.Application.Dashboard.Queries.ExportLancamentos;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Application.Dashboard.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public class ExportLancamentosExcelQueryHandler : IRequestHandler<ExportLancamentosExcelQuery, Result<byte[]>>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IContaRepository _contaRepository;
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IExcelGenerator _excelGenerator;

    public ExportLancamentosExcelQueryHandler(
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IContaRepository contaRepository,
        ICartaoRepository cartaoRepository,
        IExcelGenerator excelGenerator)
    {
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _contaRepository = contaRepository;
        _cartaoRepository = cartaoRepository;
        _excelGenerator = excelGenerator;
    }

    public async Task<Result<byte[]>> Handle(ExportLancamentosExcelQuery request, CancellationToken cancellationToken)
    {
        // Fetch lancamentos
        var lancamentos = await _lancamentoRepository.ListAsync(
            request.DataInicio,
            request.DataFim,
            request.ContaId,
            request.CartaoId,
            request.CategoriaId,
            null,
            null,
            null,
            0,
            int.MaxValue,
            cancellationToken);

        // Build DTOs
        var dtos = new List<RelatorioLancamentoDto>();
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

            var dto = new RelatorioLancamentoDto(
                lancamento.Id,
                lancamento.Descricao ?? "Lançamento",
                lancamento.Valor,
                lancamento.Data,
                lancamento.Tipo.ToString(),
                lancamento.Natureza.ToString(),
                lancamento.StatusPagamento.ToString(),
                categoriaNome,
                contaNome,
                cartaoNome,
                lancamento.DataPagamento);

            dtos.Add(dto);
        }

        // Generate Excel
        var bytes = _excelGenerator.GenerateLancamentosReport(dtos, "lancamentos.xlsx");

        return Result.Success(bytes);
    }
}
