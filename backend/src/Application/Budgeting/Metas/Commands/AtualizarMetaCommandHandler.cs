namespace BudgetCouple.Application.Budgeting.Metas.Commands;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Common;
using MediatR;

public class AtualizarMetaCommandHandler : IRequestHandler<AtualizarMetaCommand, Result<MetaDto>>
{
    private readonly IMetaRepository _metaRepository;

    public AtualizarMetaCommandHandler(IMetaRepository metaRepository)
    {
        _metaRepository = metaRepository;
    }

    public async Task<Result<MetaDto>> Handle(AtualizarMetaCommand request, CancellationToken cancellationToken)
    {
        var meta = await _metaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (meta == null)
            return Result.Failure<MetaDto>(Error.NotFound("Meta não encontrada."));

        var result = meta.Atualizar(request.Nome, request.ValorAlvo, request.DataLimite, request.PercentualAlerta);
        if (result.IsFailure)
            return Result.Failure<MetaDto>(result.Error);

        await _metaRepository.SaveChangesAsync(cancellationToken);

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
