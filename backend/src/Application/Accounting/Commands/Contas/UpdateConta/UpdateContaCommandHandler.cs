namespace BudgetCouple.Application.Accounting.Commands.Contas.UpdateConta;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class UpdateContaCommandHandler : IRequestHandler<UpdateContaCommand, Result<ContaDto>>
{
    private readonly IContaRepository _contaRepository;
    private readonly IApplicationDbContext _dbContext;

    public UpdateContaCommandHandler(IContaRepository contaRepository, IApplicationDbContext dbContext)
    {
        _contaRepository = contaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<ContaDto>> Handle(UpdateContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (conta == null)
            return Result.Failure<ContaDto>(Error.NotFound("Conta não encontrada"));

        var result = conta.Atualizar(request.Nome, request.Observacoes, request.Icone, request.CorHex);
        if (!result.IsSuccess)
            return Result.Failure<ContaDto>(result.Error);

        _contaRepository.Update(conta);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = ContaDto.FromDomain(conta, conta.SaldoInicial);
        return Result.Success(dto);
    }
}
