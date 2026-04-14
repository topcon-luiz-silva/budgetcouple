namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.DeletarAnexo;

using MediatR;

public record DeletarAnexoCommand(
    Guid LancamentoId,
    Guid AnexoId) : IRequest;
