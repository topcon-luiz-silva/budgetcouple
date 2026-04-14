namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.AdicionarAnexo;

using MediatR;

public record AdicionarAnexoCommand(
    Guid LancamentoId,
    string NomeArquivo,
    string ContentType,
    long TamanhoBytes,
    Stream ArquivoStream) : IRequest<AdicionarAnexoResponse>;

public record AdicionarAnexoResponse(
    Guid AnexoId,
    string NomeArquivo,
    long TamanhoBytes,
    DateTime EnviadoEm);
