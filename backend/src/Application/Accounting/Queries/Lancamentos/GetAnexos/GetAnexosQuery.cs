namespace BudgetCouple.Application.Accounting.Queries.Lancamentos.GetAnexos;

using MediatR;

public record GetAnexosQuery(Guid LancamentoId) : IRequest<List<AnexoDto>>;

public record AnexoDto(
    Guid Id,
    string NomeArquivo,
    string ContentType,
    long TamanhoBytes,
    DateTime EnviadoEm);
