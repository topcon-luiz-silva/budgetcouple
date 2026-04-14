namespace BudgetCouple.Domain.Accounting.Categorias;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Common;

/// <summary>
/// Aggregate root for expense/income categories with subcategories.
/// </summary>
public class Categoria : AggregateRoot
{
    public string Nome { get; private set; } = null!;
    public TipoCategoria Tipo { get; private set; }
    public string? Icone { get; private set; }
    public string Cor { get; private set; } = "#000000";
    public bool Sistema { get; private set; } // Cannot be deleted or type changed if true
    public bool Ativa { get; private set; } = true;
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    // EF navigation
    private readonly List<Subcategoria> _subcategorias = new();
    public IReadOnlyList<Subcategoria> Subcategorias => _subcategorias.AsReadOnly();

    // EF constructor
    protected Categoria() { }

    public static Result<Categoria> Create(
        string nome,
        TipoCategoria tipo,
        string? icone = null,
        string? cor = null,
        bool sistema = false)
    {
        // Validation: nome não vazio
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Categoria>(Error.Validation("Nome da categoria não pode estar vazio."));

        return Result.Success(new Categoria
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Tipo = tipo,
            Icone = icone,
            Cor = cor ?? "#000000",
            Sistema = sistema,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Factory for seeding seed data with fixed GUID (for database seeding only).
    /// </summary>
    public static Categoria CreateForSeed(Guid id, string nome, TipoCategoria tipo, string icone, string cor, bool sistema = false)
    {
        return new Categoria
        {
            Id = id,
            Nome = nome,
            Tipo = tipo,
            Icone = icone,
            Cor = cor,
            Sistema = sistema,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };
    }

    public Result Renomear(string novoNome)
    {
        // Validation: nome não vazio
        if (string.IsNullOrWhiteSpace(novoNome))
            return Result.Failure(Error.Validation("Nome da categoria não pode estar vazio."));

        // Cannot rename sistema categories
        if (Sistema)
            return Result.Failure(Error.Conflict("Categoria do sistema não pode ser renomeada."));

        Nome = novoNome;
        AtualizadoEm = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Desativar()
    {
        // Cannot deactivate sistema categories
        if (Sistema)
            return Result.Failure(Error.Conflict("Categoria do sistema não pode ser desativada."));

        Ativa = false;
        AtualizadoEm = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AdicionarSubcategoria(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(Error.Validation("Nome da subcategoria não pode estar vazio."));

        var subcategoria = Subcategoria.Create(nome);
        _subcategorias.Add(subcategoria);
        AtualizadoEm = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AtualizarSubcategoria(Guid subcategoriaId, string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            return Result.Failure(Error.Validation("Nome da subcategoria não pode estar vazio."));

        var subcategoria = _subcategorias.FirstOrDefault(s => s.Id == subcategoriaId);
        if (subcategoria == null)
            return Result.Failure(Error.NotFound("Subcategoria não encontrada."));

        subcategoria.Renomear(novoNome);
        AtualizadoEm = DateTime.UtcNow;
        return Result.Success();
    }

    public Result DesativarSubcategoria(Guid subcategoriaId)
    {
        var subcategoria = _subcategorias.FirstOrDefault(s => s.Id == subcategoriaId);
        if (subcategoria == null)
            return Result.Failure(Error.NotFound("Subcategoria não encontrada."));

        subcategoria.Desativar();
        AtualizadoEm = DateTime.UtcNow;
        return Result.Success();
    }
}
