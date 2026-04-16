using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

namespace BudgetCouple.Application.Accounting.Commands.Faturas.EstornarFatura;

public record EstornarFaturaCommand(
    Guid CartaoId,
    string Competencia // YYYY-MM
) : IRequest<Result<FaturaDto>>;
