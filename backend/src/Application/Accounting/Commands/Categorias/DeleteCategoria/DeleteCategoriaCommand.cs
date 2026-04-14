namespace BudgetCouple.Application.Accounting.Commands.Categorias.DeleteCategoria;

using BudgetCouple.Domain.Common;
using MediatR;

public record DeleteCategoriaCommand(Guid Id) : IRequest<Result>;
