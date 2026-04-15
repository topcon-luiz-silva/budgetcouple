namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateParcelado;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Common;
using MediatR;

public class CreateLancamentoParceladoCommandHandler : IRequestHandler<CreateLancamentoParceladoCommand, Result<List<LancamentoDto>>>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IContaRepository _contaRepository;
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IApplicationDbContext _dbContext;

    public CreateLancamentoParceladoCommandHandler(
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

    public async Task<Result<List<LancamentoDto>>> Handle(CreateLancamentoParceladoCommand request, CancellationToken cancellationToken)
    {
        // Validate categoria exists
        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);
        if (categoria == null)
            return Result.Failure<List<LancamentoDto>>(Error.NotFound($"Categoria com ID {request.CategoriaId} não encontrada"));

        // Validate conta exists if provided
        string? contaNome = null;
        if (request.ContaId.HasValue)
        {
            var conta = await _contaRepository.GetByIdAsync(request.ContaId.Value, cancellationToken);
            if (conta == null)
                return Result.Failure<List<LancamentoDto>>(Error.NotFound($"Conta com ID {request.ContaId} não encontrada"));
            contaNome = conta.Nome;
        }

        // Validate cartao exists if provided
        string? cartaoNome = null;
        if (request.CartaoId.HasValue)
        {
            var cartao = await _cartaoRepository.GetByIdAsync(request.CartaoId.Value, cancellationToken);
            if (cartao == null)
                return Result.Failure<List<LancamentoDto>>(Error.NotFound($"Cartão com ID {request.CartaoId} não encontrada"));
            cartaoNome = cartao.Nome;
        }

        // Map request.NaturezaLancamento (RECEITA|DESPESA|TRANSFERENCIA) → TipoLancamento
        var tipo = request.NaturezaLancamento == "RECEITA"
            ? TipoLancamento.RECEITA
            : TipoLancamento.DESPESA;

        // Parcelados são sempre PREVISTA (previstos para os próximos meses)
        var natureza = NaturezaLancamento.PREVISTA;
        var result = Lancamento.CriarParcelado(
            tipo,
            natureza,
            request.ValorTotal,
            request.DataPrimeiraParcela,
            request.TotalParcelas,
            request.CategoriaId,
            request.SubcategoriaId,
            request.ContaId,
            request.CartaoId,
            request.DescricaoBase);

        if (!result.IsSuccess)
            return Result.Failure<List<LancamentoDto>>(result.Error);

        var lancamentos = result.Value;

        // Set tags if provided
        if (request.Tags != null && request.Tags.Count > 0)
        {
            foreach (var lancamento in lancamentos)
            {
                lancamento.Tags.AddRange(request.Tags);
            }
        }

        // Marcar transferências com tag interna para que dashboard possa excluí-las
        if (request.NaturezaLancamento == "TRANSFERENCIA")
        {
            foreach (var lancamento in lancamentos)
            {
                if (!lancamento.Tags.Contains("__TRANSFERENCIA__"))
                    lancamento.Tags.Add("__TRANSFERENCIA__");
            }
        }

        _lancamentoRepository.AddRange(lancamentos);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dtos = lancamentos.Select(l => LancamentoDto.FromDomain(l, contaNome, cartaoNome, categoria.Nome)).ToList();
        return Result.Success(dtos);
    }
}
