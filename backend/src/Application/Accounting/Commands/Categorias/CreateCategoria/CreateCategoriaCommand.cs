namespace BudgetCouple.Application.Accounting.Commands.Categorias.CreateCategoria;

using BudgetCouple.Application.Accounting.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record CreateCategoriaCommand(
    string Nome,
    string TipoCategoria,
    string CorHex,
    string? Icone,
    Guid? ParentCategoriaId
) : IRequest<Result<CategoriaDto>>;
