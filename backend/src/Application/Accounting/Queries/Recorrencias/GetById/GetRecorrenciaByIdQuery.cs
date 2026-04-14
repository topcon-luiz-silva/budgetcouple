namespace BudgetCouple.Application.Accounting.Queries.Recorrencias.GetById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record GetRecorrenciaByIdQuery(Guid Id) : IRequest<Result<RecorrenciaDto>>;
