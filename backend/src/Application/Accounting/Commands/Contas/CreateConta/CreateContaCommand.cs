namespace BudgetCouple.Application.Accounting.Commands.Contas.CreateConta;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record CreateContaCommand(
    string Nome,
    string TipoConta,
    decimal SaldoInicial,
    string CorHex,
    string? Icone,
    string? Observacoes
) : IRequest<Result<ContaDto>>;
