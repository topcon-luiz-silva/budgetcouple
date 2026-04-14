namespace BudgetCouple.Application.Accounting.Queries.Categorias.ListCategorias;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record ListCategoriasQuery : IRequest<Result<List<CategoriaDto>>>;
