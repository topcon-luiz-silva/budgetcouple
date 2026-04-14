namespace BudgetCouple.Application.Accounting.Commands.Cartoes.DeleteCartao;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class DeleteCartaoCommandHandler : IRequestHandler<DeleteCartaoCommand, Result>
{
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IApplicationDbContext _dbContext;

    public DeleteCartaoCommandHandler(ICartaoRepository cartaoRepository, IApplicationDbContext dbContext)
    {
        _cartaoRepository = cartaoRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteCartaoCommand request, CancellationToken cancellationToken)
    {
        var cartao = await _cartaoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cartao == null)
            return Result.Failure(Error.NotFound("Cartão não encontrado"));

        _cartaoRepository.Delete(cartao);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
