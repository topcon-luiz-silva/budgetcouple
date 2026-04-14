namespace BudgetCouple.Application.Accounting.Commands.Contas.UpdateConta;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record UpdateContaCommand(
    Guid Id,
    string Nome,
    string CorHex,
    string? Icone,
    string? Observacoes
) : IRequest<Result<ContaDto>>;
