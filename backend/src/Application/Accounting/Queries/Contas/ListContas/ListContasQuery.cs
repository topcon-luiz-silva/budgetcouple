namespace BudgetCouple.Application.Accounting.Queries.Contas.ListContas;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record ListContasQuery : IRequest<Result<List<ContaDto>>>;
