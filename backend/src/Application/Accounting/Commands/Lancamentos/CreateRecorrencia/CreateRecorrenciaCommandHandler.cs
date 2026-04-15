namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.CreateRecorrencia;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Accounting.Recorrencias;
using BudgetCouple.Domain.Common;
using MediatR;
using System.Text.Json;

public class CreateRecorrenciaCommandHandler : IRequestHandler<CreateRecorrenciaCommand, Result<CreateRecorrenciaResponse>>
{
    private readonly IRecorrenciaRepository _recorrenciaRepository;
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IContaRepository _contaRepository;
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IApplicationDbContext _dbContext;

    public CreateRecorrenciaCommandHandler(
        IRecorrenciaRepository recorrenciaRepository,
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IContaRepository contaRepository,
        ICartaoRepository cartaoRepository,
        IApplicationDbContext dbContext)
    {
        _recorrenciaRepository = recorrenciaRepository;
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _contaRepository = contaRepository;
        _cartaoRepository = cartaoRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<CreateRecorrenciaResponse>> Handle(CreateRecorrenciaCommand request, CancellationToken cancellationToken)
    {
        // Validate categoria exists
        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);
        if (categoria == null)
            return Result.Failure<CreateRecorrenciaResponse>(Error.NotFound($"Categoria com ID {request.CategoriaId} não encontrada"));

        // Validate conta exists if provided
        string? contaNome = null;
        if (request.ContaId.HasValue)
        {
            var conta = await _contaRepository.GetByIdAsync(request.ContaId.Value, cancellationToken);
            if (conta == null)
                return Result.Failure<CreateRecorrenciaResponse>(Error.NotFound($"Conta com ID {request.ContaId} não encontrada"));
            contaNome = conta.Nome;
        }

        // Validate cartao exists if provided
        string? cartaoNome = null;
        if (request.CartaoId.HasValue)
        {
            var cartao = await _cartaoRepository.GetByIdAsync(request.CartaoId.Value, cancellationToken);
            if (cartao == null)
                return Result.Failure<CreateRecorrenciaResponse>(Error.NotFound($"Cartão com ID {request.CartaoId} não encontrada"));
            cartaoNome = cartao.Nome;
        }

        // Parse enums
        if (!Enum.TryParse<FrequenciaRecorrencia>(request.Frequencia, out var frequencia))
            return Result.Failure<CreateRecorrenciaResponse>(Error.Validation("Frequência inválida"));

        if (!Enum.TryParse<NaturezaLancamento>(request.NaturezaLancamento, out var natureza))
            return Result.Failure<CreateRecorrenciaResponse>(Error.Validation("Natureza do lançamento inválida"));

        // Create template JSON
        var template = new
        {
            descricaoBase = request.DescricaoBase,
            valor = request.ValorBase,
            categoriaId = request.CategoriaId,
            subcategoriaId = request.SubcategoriaId,
            contaId = request.ContaId,
            cartaoId = request.CartaoId,
            natureza = request.NaturezaLancamento,
            tags = request.Tags ?? new List<string>()
        };

        var templateJson = JsonSerializer.Serialize(template);

        // Create recorrencia
        var recorrenciaResult = Recorrencia.Create(frequencia, request.DataInicio, request.DataFim, templateJson);
        if (!recorrenciaResult.IsSuccess)
            return Result.Failure<CreateRecorrenciaResponse>(recorrenciaResult.Error);

        var recorrencia = recorrenciaResult.Value;
        _recorrenciaRepository.Add(recorrencia);

        // Generate occurrences
        var gerarAte = request.GerarOcorrenciasAte ?? DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(6);
        var snapshots = recorrencia.GerarProximasOcorrencias(gerarAte);

        var tipo = request.NaturezaLancamento == "RECEITA" ? TipoLancamento.RECEITA : TipoLancamento.DESPESA;
        var lancamentos = new List<Lancamento>();

        foreach (var snapshot in snapshots)
        {
            var lancResult = Lancamento.CriarRecorrenciaOcorrencia(
                tipo,
                natureza,
                request.ValorBase,
                snapshot.DataOcorrencia,
                request.CategoriaId,
                recorrencia.Id,
                request.SubcategoriaId,
                request.ContaId,
                request.CartaoId,
                request.DescricaoBase);

            if (lancResult.IsSuccess)
            {
                var lanc = lancResult.Value;
                if (request.Tags != null && request.Tags.Count > 0)
                    lanc.Tags.AddRange(request.Tags);
                lancamentos.Add(lanc);
            }
        }

        _lancamentoRepository.AddRange(lancamentos);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var recorrenciaDto = RecorrenciaDto.FromDomain(recorrencia);
        var lancamentosDto = lancamentos.Select(l => LancamentoDto.FromDomain(l, contaNome, cartaoNome, categoria.Nome)).ToList();

        return Result.Success(new CreateRecorrenciaResponse(recorrenciaDto, lancamentosDto));
    }
}
