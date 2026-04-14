namespace BudgetCouple.Application.Accounting.Queries.Lancamentos.GetAnexoById;

using BudgetCouple.Application.Common.Interfaces.Accounting;
using MediatR;

public class GetAnexoByIdHandler : IRequestHandler<GetAnexoByIdQuery, AnexoDetailDto>
{
    private readonly ILancamentoAnexoRepository _anexoRepository;

    public GetAnexoByIdHandler(ILancamentoAnexoRepository anexoRepository)
    {
        _anexoRepository = anexoRepository;
    }

    public async Task<AnexoDetailDto> Handle(GetAnexoByIdQuery request, CancellationToken cancellationToken)
    {
        var anexo = await _anexoRepository.GetByIdAsync(request.AnexoId, cancellationToken);

        if (anexo == null || anexo.LancamentoId != request.LancamentoId)
            throw new InvalidOperationException($"Anexo {request.AnexoId} não encontrado.");

        return new AnexoDetailDto(
            anexo.Id,
            anexo.NomeArquivo,
            anexo.ContentType,
            anexo.TamanhoBytes,
            anexo.CaminhoStorage,
            anexo.EnviadoEm);
    }
}
