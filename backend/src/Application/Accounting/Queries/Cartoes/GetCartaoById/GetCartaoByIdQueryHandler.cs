namespace BudgetCouple.Application.Accounting.Queries.Cartoes.GetCartaoById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class GetCartaoByIdQueryHandler : IRequestHandler<GetCartaoByIdQuery, Result<CartaoDto>>
{
    private readonly ICartaoRepository _cartaoRepository;

    public GetCartaoByIdQueryHandler(ICartaoRepository cartaoRepository)
    {
        _cartaoRepository = cartaoRepository;
    }

    public async Task<Result<CartaoDto>> Handle(GetCartaoByIdQuery request, CancellationToken cancellationToken)
    {
        var cartao = await _cartaoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cartao == null)
            return Result.Failure<CartaoDto>(Error.NotFound("Cartão não encontrado"));

        var dto = CartaoDto.FromDomain(cartao);
        return Result.Success(dto);
    }
}
