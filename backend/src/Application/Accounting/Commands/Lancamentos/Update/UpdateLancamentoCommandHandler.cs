namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.Update;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class UpdateLancamentoCommandHandler : IRequestHandler<UpdateLancamentoCommand, Result<LancamentoDto>>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _dbContext;

    public UpdateLancamentoCommandHandler(
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository,
        IApplicationDbContext dbContext)
    {
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<LancamentoDto>> Handle(UpdateLancamentoCommand request, CancellationToken cancellationToken)
    {
        // Get lancamento
        var lancamento = await _lancamentoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lancamento == null)
            return Result.Failure<LancamentoDto>(Error.NotFound($"Lançamento com ID {request.Id} não encontrado"));

        // Validate categoria exists
        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);
        if (categoria == null)
            return Result.Failure<LancamentoDto>(Error.NotFound($"Categoria com ID {request.CategoriaId} não encontrada"));

        // Update lancamento
        var resultado = lancamento.AtualizarDados(
            lancamento.Tipo,
            lancamento.Natureza,
            request.Valor,
            request.DataCompetencia,
            request.CategoriaId,
            request.SubcategoriaId,
            request.Descricao);

        if (!resultado.IsSuccess)
            return Result.Failure<LancamentoDto>(resultado.Error);

        // Update tags
        if (request.Tags != null)
        {
            lancamento.Tags.Clear();
            lancamento.Tags.AddRange(request.Tags);
        }

        _lancamentoRepository.Update(lancamento);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = LancamentoDto.FromDomain(lancamento, categoriaNome: categoria.Nome);
        return Result.Success(dto);
    }
}
