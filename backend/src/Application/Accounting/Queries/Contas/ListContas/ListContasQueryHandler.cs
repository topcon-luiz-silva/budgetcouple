namespace BudgetCouple.Application.Accounting.Queries.Contas.ListContas;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class ListContasQueryHandler : IRequestHandler<ListContasQuery, Result<List<ContaDto>>>
{
    private readonly IContaRepository _contaRepository;

    public ListContasQueryHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<Result<List<ContaDto>>> Handle(ListContasQuery request, CancellationToken cancellationToken)
    {
        var contas = await _contaRepository.ListAsync(cancellationToken);
        var dtos = contas.Select(c => ContaDto.FromDomain(c, c.SaldoInicial)).ToList();
        return Result.Success(dtos);
    }
}
