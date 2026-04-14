namespace BudgetCouple.Application.Accounting.Commands.Contas.CreateConta;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Contas;
using BudgetCouple.Domain.Common;
using MediatR;

public class CreateContaCommandHandler : IRequestHandler<CreateContaCommand, Result<ContaDto>>
{
    private readonly IContaRepository _contaRepository;
    private readonly IApplicationDbContext _dbContext;

    public CreateContaCommandHandler(IContaRepository contaRepository, IApplicationDbContext dbContext)
    {
        _contaRepository = contaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<ContaDto>> Handle(CreateContaCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<TipoConta>(request.TipoConta, out var tipoConta))
        {
            return Result.Failure<ContaDto>(Error.Validation("Tipo de conta inválido"));
        }

        var result = Conta.Create(
            request.Nome,
            tipoConta,
            request.SaldoInicial,
            request.Observacoes,
            request.Icone,
            request.CorHex);

        if (!result.IsSuccess)
            return Result.Failure<ContaDto>(result.Error);

        var conta = result.Value;
        _contaRepository.Add(conta);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = ContaDto.FromDomain(conta, conta.SaldoInicial);
        return Result.Success(dto);
    }
}
