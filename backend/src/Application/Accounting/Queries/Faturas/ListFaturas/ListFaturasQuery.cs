using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

namespace BudgetCouple.Application.Accounting.Queries.Faturas.ListFaturas;

public record ListFaturasQuery(
    Guid CartaoId
) : IRequest<Result<List<FaturaResumoDto>>>;
