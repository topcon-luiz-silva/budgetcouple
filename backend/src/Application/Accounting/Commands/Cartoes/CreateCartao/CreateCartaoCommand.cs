namespace BudgetCouple.Application.Accounting.Commands.Cartoes.CreateCartao;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record CreateCartaoCommand(
    string Nome,
    string Bandeira,
    string? UltimosDigitos,
    decimal Limite,
    int DiaFechamento,
    int DiaVencimento,
    Guid ContaPagamentoId,
    string CorHex,
    string? Icone
) : IRequest<Result<CartaoDto>>;
