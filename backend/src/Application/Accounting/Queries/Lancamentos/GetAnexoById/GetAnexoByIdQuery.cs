namespace BudgetCouple.Application.Accounting.Queries.Lancamentos.GetAnexoById;

using MediatR;

public record GetAnexoByIdQuery(
    Guid LancamentoId,
    Guid AnexoId) : IRequest<AnexoDetailDto>;

public record AnexoDetailDto(
    Guid Id,
    string NomeArquivo,
    string ContentType,
    long TamanhoBytes,
    string CaminhoStorage,
    DateTime EnviadoEm);
