namespace BudgetCouple.Application.Accounting.Queries.Cartoes.ListCartoes;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class ListCartoesQueryHandler : IRequestHandler<ListCartoesQuery, Result<List<CartaoDto>>>
{
    private readonly ICartaoRepository _cartaoRepository;

    public ListCartoesQueryHandler(ICartaoRepository cartaoRepository)
    {
        _cartaoRepository = cartaoRepository;
    }

    public async Task<Result<List<CartaoDto>>> Handle(ListCartoesQuery request, CancellationToken cancellationToken)
    {
        var cartoes = await _cartaoRepository.ListAsync(cancellationToken);
        var dtos = cartoes.Select(c => CartaoDto.FromDomain(c)).ToList();
        return Result.Success(dtos);
    }
}
