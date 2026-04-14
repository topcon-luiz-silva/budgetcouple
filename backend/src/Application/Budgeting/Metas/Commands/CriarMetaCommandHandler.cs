namespace BudgetCouple.Application.Budgeting.Metas.Commands;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Application.Common.Interfaces.Budgeting;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Common;
using MediatR;

public class CriarMetaCommandHandler : IRequestHandler<CriarMetaCommand, Result<MetaDto>>
{
    private readonly IMetaRepository _metaRepository;

    public CriarMetaCommandHandler(IMetaRepository metaRepository)
    {
        _metaRepository = metaRepository;
    }

    public async Task<Result<MetaDto>> Handle(CriarMetaCommand request, CancellationToken cancellationToken)
    {
        Meta? meta = null;
        Error? error = null;

        if (request.Tipo == "ECONOMIA")
        {
            var result = MetaEconomia.Create(
                request.Nome,
                request.ValorAlvo,
                request.DataLimite,
                request.PercentualAlerta);

            if (result.IsFailure)
                return Result.Failure<MetaDto>(result.Error);

            meta = result.Value;
        }
        else if (request.Tipo == "REDUCAO_CATEGORIA")
        {
            var result = MetaReducaoCategoria.Create(
                request.Nome,
                request.ValorAlvo,
                request.CategoriaId ?? Guid.Empty,
                request.DataLimite,
                request.Periodo,
                request.PercentualAlerta);

            if (result.IsFailure)
                return Result.Failure<MetaDto>(result.Error);

            meta = result.Value;
        }
        else
        {
            error = Error.Validation($"Tipo de meta inválido: {request.Tipo}");
            return Result.Failure<MetaDto>(error);
        }

        await _metaRepository.AddAsync(meta, cancellationToken);
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
