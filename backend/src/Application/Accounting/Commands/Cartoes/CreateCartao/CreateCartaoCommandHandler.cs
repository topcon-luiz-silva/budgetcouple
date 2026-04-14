namespace BudgetCouple.Application.Accounting.Commands.Cartoes.CreateCartao;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Cartoes;
using BudgetCouple.Domain.Common;
using MediatR;

public class CreateCartaoCommandHandler : IRequestHandler<CreateCartaoCommand, Result<CartaoDto>>
{
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IContaRepository _contaRepository;
    private readonly IApplicationDbContext _dbContext;

    public CreateCartaoCommandHandler(
        ICartaoRepository cartaoRepository,
        IContaRepository contaRepository,
        IApplicationDbContext dbContext)
    {
        _cartaoRepository = cartaoRepository;
        _contaRepository = contaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<CartaoDto>> Handle(CreateCartaoCommand request, CancellationToken cancellationToken)
    {
        // Verify that the account exists
        var conta = await _contaRepository.GetByIdAsync(request.ContaPagamentoId, cancellationToken);
        if (conta == null)
            return Result.Failure<CartaoDto>(Error.NotFound("Conta de pagamento não encontrada"));

        var result = Cartao.Create(
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

        var cartao = result.Value;
        _cartaoRepository.Add(cartao);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = CartaoDto.FromDomain(cartao, request.UltimosDigitos);
        return Result.Success(dto);
    }
}
