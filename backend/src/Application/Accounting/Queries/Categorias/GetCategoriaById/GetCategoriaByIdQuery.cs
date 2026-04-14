namespace BudgetCouple.Application.Accounting.Queries.Categorias.GetCategoriaById;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record GetCategoriaByIdQuery(Guid Id) : IRequest<Result<CategoriaDto>>;
