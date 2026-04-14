namespace BudgetCouple.Domain.Budgeting.Metas;

using BudgetCouple.Domain.Common;

/// <summary>
/// Base class for savings/reduction goals.
/// Uses Table Per Hierarchy (TPH) with Type discriminator.
/// </summary>
public abstract class Meta : AggregateRoot
{
    public string Tipo { get; protected set; } = null!; // ECONOMIA, REDUCAO_CATEGORIA
    public string Nome { get; protected set; } = null!;
    public decimal ValorAlvo { get; protected set; }
    public decimal ValorAtual { get; protected set; }
    public DateOnly? DataLimite { get; protected set; }
    public Guid? CategoriaId { get; protected set; } // Null for ECONOMIA, set for REDUCAO_CATEGORIA
    public string? Periodo { get; protected set; } // MENSAL, TRIMESTRAL, ANUAL (optional)
    public int PercentualAlerta { get; protected set; } = 80;
    public bool Ativa { get; protected set; } = true;
    public DateTime CriadoEm { get; protected set; }
    public DateTime AtualizadoEm { get; protected set; }

    // EF constructor
    protected Meta() { }

    protected Meta(string nome, decimal valorAlvo, DateOnly? dataLimite, int percentualAlerta)
    {
        Nome = nome;
        ValorAlvo = valorAlvo;
        ValorAtual = 0;
        DataLimite = dataLimite;
        PercentualAlerta = percentualAlerta;
        Ativa = true;
        CriadoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }

    public virtual Result Atualizar(string nome, decimal valorAlvo, DateOnly? dataLimite, int percentualAlerta)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(Error.Validation("Nome da meta não pode estar vazio."));

        if (valorAlvo <= 0)
            return Result.Failure(Error.Validation("Valor alvo deve ser maior que zero."));

        if (percentualAlerta < 1 || percentualAlerta > 100)
            return Result.Failure(Error.Validation("Percentual de alerta deve estar entre 1 e 100."));

        Nome = nome;
        ValorAlvo = valorAlvo;
        DataLimite = dataLimite;
        PercentualAlerta = percentualAlerta;
        AtualizadoEm = DateTime.UtcNow;

        return Result.Success();
    }

    public void AtualizarValorAtual(decimal novoValor)
    {
        ValorAtual = novoValor;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativa = false;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativa = true;
        AtualizadoEm = DateTime.UtcNow;
    }

    public bool AtingiuAlerta()
    {
        return ValorAtual >= (ValorAlvo * PercentualAlerta / 100m);
    }

    public bool Atingida()
    {
        return ValorAtual >= ValorAlvo;
    }

    public decimal PercentualProgresso()
    {
        if (ValorAlvo == 0)
            return 0;
        return (ValorAtual / ValorAlvo) * 100;
    }
}

/// <summary>
/// Meta de economia (savings goal).
/// Target: Save a specific amount by a given date.
/// </summary>
public class MetaEconomia : Meta
{
    public static Result<MetaEconomia> Create(
        string nome,
        decimal valorAlvo,
        DateOnly? dataLimite = null,
        int percentualAlerta = 80)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<MetaEconomia>(Error.Validation("Nome da meta não pode estar vazio."));

        if (valorAlvo <= 0)
            return Result.Failure<MetaEconomia>(Error.Validation("Valor alvo deve ser maior que zero."));

        if (percentualAlerta < 1 || percentualAlerta > 100)
            return Result.Failure<MetaEconomia>(Error.Validation("Percentual de alerta deve estar entre 1 e 100."));

        return Result.Success(new MetaEconomia
        {
            Id = Guid.NewGuid(),
            Tipo = "ECONOMIA",
            Nome = nome,
            ValorAlvo = valorAlvo,
            ValorAtual = 0,
            DataLimite = dataLimite,
            CategoriaId = null,
            Periodo = null,
            PercentualAlerta = percentualAlerta,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        });
    }
}

/// <summary>
/// Meta de redução de categoria (reduce category spending).
/// Target: Spend less than a target amount in a specific category.
/// </summary>
public class MetaReducaoCategoria : Meta
{
    public static Result<MetaReducaoCategoria> Create(
        string nome,
        decimal valorAlvo,
        Guid categoriaId,
        DateOnly? dataLimite = null,
        string? periodo = null,
        int percentualAlerta = 80)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<MetaReducaoCategoria>(Error.Validation("Nome da meta não pode estar vazio."));

        if (valorAlvo <= 0)
            return Result.Failure<MetaReducaoCategoria>(Error.Validation("Valor alvo deve ser maior que zero."));

        if (categoriaId == Guid.Empty)
            return Result.Failure<MetaReducaoCategoria>(Error.Validation("Categoria ID inválido."));

        if (percentualAlerta < 1 || percentualAlerta > 100)
            return Result.Failure<MetaReducaoCategoria>(Error.Validation("Percentual de alerta deve estar entre 1 e 100."));

        return Result.Success(new MetaReducaoCategoria
        {
            Id = Guid.NewGuid(),
            Tipo = "REDUCAO_CATEGORIA",
            Nome = nome,
            ValorAlvo = valorAlvo,
            ValorAtual = 0,
            DataLimite = dataLimite,
            CategoriaId = categoriaId,
            Periodo = periodo,
            PercentualAlerta = percentualAlerta,
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        });
    }
}
