namespace BudgetCouple.Application.Accounting.Queries.Lancamentos.GetById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class GetLancamentoByIdQueryHandler : IRequestHandler<GetLancamentoByIdQuery, Result<LancamentoDto>>
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public GetLancamentoByIdQueryHandler(
        ILancamentoRepository lancamentoRepository,
        ICategoriaRepository categoriaRepository)
    {
        _lancamentoRepository = lancamentoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Result<LancamentoDto>> Handle(GetLancamentoByIdQuery request, CancellationToken cancellationToken)
    {
        var lancamento = await _lancamentoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lancamento == null)
            return Result.Failure<LancamentoDto>(Error.NotFound($"Lançamento com ID {request.Id} não encontrado"));

        var categoria = await _categoriaRepository.GetByIdAsync(lancamento.CategoriaId, cancellationToken);
        var categoriaNome = categoria?.Nome ?? "";

        var dto = LancamentoDto.FromDomain(lancamento, categoriaNome: categoriaNome);
        return Result.Success(dto);
    }
}
