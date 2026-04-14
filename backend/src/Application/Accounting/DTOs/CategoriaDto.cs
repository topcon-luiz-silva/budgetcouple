namespace BudgetCouple.Application.Accounting.DTOs;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Categorias;

/// <summary>
/// DTO for Categoria aggregate.
/// </summary>
public record CategoriaDto(
    Guid Id,
    string Nome,
    TipoCategoria TipoCategoria,
    string CorHex,
    string? Icone,
    bool Sistema,
    bool Ativa,
    DateTime CriadoEm)
{
    public static CategoriaDto FromDomain(Categoria categoria)
    {
        return new CategoriaDto(
            categoria.Id,
            categoria.Nome,
            categoria.Tipo,
            categoria.Cor,
            categoria.Icone,
            categoria.Sistema,
            categoria.Ativa,
            categoria.CriadoEm);
    }
}
