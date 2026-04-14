namespace BudgetCouple.Application.Accounting.Commands.Categorias.UpdateCategoria;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record UpdateCategoriaCommand(
    Guid Id,
    string Nome,
    string CorHex,
    string? Icone
) : IRequest<Result<CategoriaDto>>;
