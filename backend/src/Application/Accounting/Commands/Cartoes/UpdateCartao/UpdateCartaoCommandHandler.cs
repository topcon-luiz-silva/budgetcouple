namespace BudgetCouple.Application.Accounting.Commands.Cartoes.UpdateCartao;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class UpdateCartaoCommandHandler : IRequestHandler<UpdateCartaoCommand, Result<CartaoDto>>
{
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IContaRepository _contaRepository;
    private readonly IApplicationDbContext _dbContext;

    public UpdateCartaoCommandHandler(
        ICartaoRepository cartaoRepository,
        IContaRepository contaRepository,
        IApplicationDbContext dbContext)
    {
        _cartaoRepository = cartaoRepository;
        _contaRepository = contaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<CartaoDto>> Handle(UpdateCartaoCommand request, CancellationToken cancellationToken)
    {
        var cartao = await _cartaoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cartao == null)
            return Result.Failure<CartaoDto>(Error.NotFound("Cartão não encontrado"));

        // Verify that the account exists
        var conta = await _contaRepository.GetByIdAsync(request.ContaPagamentoId, cancellationToken);
        if (conta == null)
            return Result.Failure<CartaoDto>(Error.NotFound("Conta de pagamento não encontrada"));

        var result = cartao.Atualizar(
            request.Nome,
            request.Bandeira,
            request.DiaFechamento,
            request.DiaVencimento,
            request.Limite,
            request.ContaPagamentoId,
            request.Icone,
            request.CorHex);

        if (!result.IsSuccess)
            return Result.Failure<CartaoDto>(result.Error);

        _cartaoRepository.Update(cartao);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = CartaoDto.FromDomain(cartao, request.UltimosDigitos);
        return Result.Success(dto);
    }
}
