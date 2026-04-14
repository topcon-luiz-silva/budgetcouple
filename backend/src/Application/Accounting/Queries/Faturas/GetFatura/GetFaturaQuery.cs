using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

namespace BudgetCouple.Application.Accounting.Queries.Faturas.GetFatura;

public record GetFaturaQuery(
    Guid CartaoId,
    string Competencia // YYYY-MM
) : IRequest<Result<FaturaDto>>;
