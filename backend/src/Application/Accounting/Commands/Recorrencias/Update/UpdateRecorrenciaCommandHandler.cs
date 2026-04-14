namespace BudgetCouple.Application.Accounting.Commands.Recorrencias.Update;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Common;
using MediatR;

public class UpdateRecorrenciaCommandHandler : IRequestHandler<UpdateRecorrenciaCommand, Result<RecorrenciaDto>>
{
    private readonly IRecorrenciaRepository _recorrenciaRepository;
    private readonly IApplicationDbContext _dbContext;

    public UpdateRecorrenciaCommandHandler(
        IRecorrenciaRepository recorrenciaRepository,
        IApplicationDbContext dbContext)
    {
        _recorrenciaRepository = recorrenciaRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<RecorrenciaDto>> Handle(UpdateRecorrenciaCommand request, CancellationToken cancellationToken)
    {
        var recorrencia = await _recorrenciaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (recorrencia == null)
            return Result.Failure<RecorrenciaDto>(Error.NotFound($"Recorrência com ID {request.Id} não encontrada"));

        // Only update DataFim if provided
        if (request.DataFim.HasValue)
        {
            if (request.DataFim.Value <= recorrencia.DataInicio)
                return Result.Failure<RecorrenciaDto>(Error.Validation("Data fim deve ser após data início"));
        }

        _recorrenciaRepository.Update(recorrencia);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = RecorrenciaDto.FromDomain(recorrencia);
        return Result.Success(dto);
    }
}
