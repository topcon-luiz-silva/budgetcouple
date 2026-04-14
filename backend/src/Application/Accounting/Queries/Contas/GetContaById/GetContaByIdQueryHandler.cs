namespace BudgetCouple.Application.Accounting.Queries.Contas.GetContaById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class GetContaByIdQueryHandler : IRequestHandler<GetContaByIdQuery, Result<ContaDto>>
{
    private readonly IContaRepository _contaRepository;

    public GetContaByIdQueryHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<Result<ContaDto>> Handle(GetContaByIdQuery request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (conta == null)
            return Result.Failure<ContaDto>(Error.NotFound("Conta não encontrada"));

        var dto = ContaDto.FromDomain(conta, conta.SaldoInicial);
        return Result.Success(dto);
    }
}
