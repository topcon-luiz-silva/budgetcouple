namespace BudgetCouple.Application.Accounting.Queries.Recorrencias.List;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record ListRecorrenciasQuery : IRequest<Result<List<RecorrenciaDto>>>;
