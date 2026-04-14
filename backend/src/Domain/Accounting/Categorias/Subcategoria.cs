namespace BudgetCouple.Domain.Accounting.Categorias;

using BudgetCouple.Domain.Common;

/// <summary>
/// Entity representing a subcategory within a category.
/// </summary>
public class Subcategoria : Entity
{
    public string Nome { get; private set; } = null!;
    public bool Ativa { get; private set; } = true;
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    // EF navigation
    public Guid CategoriaId { get; private set; }

    // EF constructor
    protected Subcategoria() { }

    internal static Subcategoria Create(string nome)
    {
        return new Subcategoria
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };
    }

    internal void Renomear(string novoNome)
    {
        Nome = novoNome;
        AtualizadoEm = DateTime.UtcNow;
    }

    internal void Desativar()
    {
        Ativa = false;
        AtualizadoEm = DateTime.UtcNow;
    }
}
