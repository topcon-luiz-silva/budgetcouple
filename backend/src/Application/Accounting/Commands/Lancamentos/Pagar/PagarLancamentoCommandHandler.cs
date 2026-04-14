namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Pagar;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class PagarLancamentoCommandHandler : IRequestHandler<PagarLancamentoCommand, Result<LancamentoDto>>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _dbContext;

    public PagarLancamentoCommandHandler(
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IApplicationDbContext dbContext)
    {
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<LancamentoDto>> Handle(PagarLancamentoCommand request, CancellationToken cancellationToken)
    {
        // Get lancamento
        var lancamento = await _lancamentoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lancamento == null)
            return Result.Failure<LancamentoDto>(Error.NotFound($"Lançamento com ID {request.Id} não encontrado"));

        // Get categoria
        var categoria = await _categoriaRepository.GetByIdAsync(lancamento.CategoriaId, cancellationToken);
        if (categoria == null)
            return Result.Failure<LancamentoDto>(Error.NotFound($"Categoria não encontrada"));

        // Mark as paid
        var resultado = lancamento.Pagar(request.DataPagamento, request.ContaDebitoId);
        if (!resultado.IsSuccess)
            return Result.Failure<LancamentoDto>(resultado.Error);

        _lancamentoRepository.Update(lancamento);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = LancamentoDto.FromDomain(lancamento, categoriaNome: categoria.Nome);
        return Result.Success(dto);
    }
}
