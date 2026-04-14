namespace BudgetCouple.Application.Accounting.Commands.Contas.DeleteConta;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class DeleteContaCommandHandler : IRequestHandler<DeleteContaCommand, Result>
{
    private readonly IContaRepository _contaRepository;
    private readonly IApplicationDbContext _dbContext;

    public DeleteContaCommandHandler(IContaRepository contaRepository, IApplicationDbContext dbContext)
    {
        _contaRepository = contaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DeleteContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (conta == null)
            return Result.Failure(Error.NotFound("Conta não encontrada"));

        _contaRepository.Delete(conta);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
