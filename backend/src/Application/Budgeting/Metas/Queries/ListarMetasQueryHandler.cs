namespace BudgetCouple.Application.Budgeting.Metas.Queries;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Common;
using MediatR;

public class ListarMetasQueryHandler : IRequestHandler<ListarMetasQuery, Result<List<MetaDto>>>
{
    private readonly IMetaRepository _metaRepository;

    public ListarMetasQueryHandler(IMetaRepository metaRepository)
    {
        _metaRepository = metaRepository;
    }

    public async Task<Result<List<MetaDto>>> Handle(ListarMetasQuery request, CancellationToken cancellationToken)
    {
        var metas = await _metaRepository.ListAsync(cancellationToken);
        var dtos = metas.Select(MapToDto).ToList();
        return Result.Success(dtos);
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
