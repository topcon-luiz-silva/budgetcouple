namespace BudgetCouple.Application.Accounting.Queries.Recorrencias.List;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class ListRecorrenciasQueryHandler : IRequestHandler<ListRecorrenciasQuery, Result<List<RecorrenciaDto>>>
{
    private readonly IRecorrenciaRepository _recorrenciaRepository;

    public ListRecorrenciasQueryHandler(IRecorrenciaRepository recorrenciaRepository)
    {
        _recorrenciaRepository = recorrenciaRepository;
    }

    public async Task<Result<List<RecorrenciaDto>>> Handle(ListRecorrenciasQuery request, CancellationToken cancellationToken)
    {
        var recorrencias = await _recorrenciaRepository.ListAsync(cancellationToken);
        var dtos = recorrencias.Select(RecorrenciaDto.FromDomain).ToList();
        return Result.Success(dtos);
    }
}
