namespace BudgetCouple.Application.Accounting.Queries.Lancamentos.GetAnexos;

using BudgetCouple.Application.Common.Interfaces.Accounting;
using MediatR;

public class GetAnexosHandler : IRequestHandler<GetAnexosQuery, List<AnexoDto>>
{
    private readonly ILancamentoAnexoRepository _anexoRepository;

    public GetAnexosHandler(ILancamentoAnexoRepository anexoRepository)
    {
        _anexoRepository = anexoRepository;
    }

    public async Task<List<AnexoDto>> Handle(GetAnexosQuery request, CancellationToken cancellationToken)
    {
        var anexos = await _anexoRepository.GetByLancamentoIdAsync(request.LancamentoId, cancellationToken);

        return anexos
            .Select(a => new AnexoDto(
                a.Id,
                a.NomeArquivo,
                a.ContentType,
                a.TamanhoBytes,
                a.EnviadoEm))
            .ToList();
    }
}
