namespace BudgetCouple.Application.Budgeting.Metas.Queries;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Common;
using MediatR;

public class ObterMetaPorIdQueryHandler : IRequestHandler<ObterMetaPorIdQuery, Result<MetaDto>>
{
    private readonly IMetaRepository _metaRepository;

    public ObterMetaPorIdQueryHandler(IMetaRepository metaRepository)
    {
        _metaRepository = metaRepository;
    }

    public async Task<Result<MetaDto>> Handle(ObterMetaPorIdQuery request, CancellationToken cancellationToken)
    {
        var meta = await _metaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (meta == null)
            return Result.Failure<MetaDto>(Error.NotFound("Meta não encontrada."));

        return Result.Success(MapToDto(meta));
    }

    private static MetaDto MapToDto(Meta meta) => new(
        meta.Id,
        meta.Tipo,
        meta.Nome,
        meta.ValorAlvo,
        meta.ValorAtual,
        meta.DataLimite,
        meta.CategoriaId,
        meta.Periodo,
        meta.PercentualAlerta,
        meta.Ativa,
        meta.CriadoEm,
        meta.AtualizadoEm);
}
