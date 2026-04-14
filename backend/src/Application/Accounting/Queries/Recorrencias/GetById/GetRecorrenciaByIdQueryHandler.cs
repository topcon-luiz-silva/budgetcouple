namespace BudgetCouple.Application.Accounting.Queries.Recorrencias.GetById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class GetRecorrenciaByIdQueryHandler : IRequestHandler<GetRecorrenciaByIdQuery, Result<RecorrenciaDto>>
{
    private readonly IRecorrenciaRepository _recorrenciaRepository;

    public GetRecorrenciaByIdQueryHandler(IRecorrenciaRepository recorrenciaRepository)
    {
        _recorrenciaRepository = recorrenciaRepository;
    }

    public async Task<Result<RecorrenciaDto>> Handle(GetRecorrenciaByIdQuery request, CancellationToken cancellationToken)
    {
        var recorrencia = await _recorrenciaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (recorrencia == null)
            return Result.Failure<RecorrenciaDto>(Error.NotFound($"Recorrência com ID {request.Id} não encontrada"));

        var dto = RecorrenciaDto.FromDomain(recorrencia);
        return Result.Success(dto);
    }
}
