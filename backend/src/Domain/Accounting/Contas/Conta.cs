namespace BudgetCouple.Domain.Accounting.Contas;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Common;

/// <summary>
/// Aggregate root for a bank account (checking, savings, wallet, investment).
/// </summary>
public class Conta : AggregateRoot
{
    public string Nome { get; private set; } = null!;
    public TipoConta Tipo { get; private set; }
    public decimal SaldoInicial { get; private set; }
    public string? Descricao { get; private set; }
    public string? Icone { get; private set; } // lucide icon name
    public string Cor { get; private set; } = "#000000"; // hex color
    public bool Ativa { get; private set; } = true;
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    // EF constructor
    protected Conta() { }

    public static Result<Conta> Create(
        string nome,
        TipoConta tipo,
        decimal saldoInicial,
        string? descricao = null,
        string? icone = null,
        string? cor = null)
    {
        // Validation: nome não vazio
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Conta>(Error.Validation("Nome da conta não pode estar vazio."));

        // Validation: saldoInicial >= 0
        if (saldoInicial < 0)
            return Result.Failure<Conta>(Error.Validation("Saldo inicial não pode ser negativo."));

        return Result.Success(new Conta
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Tipo = tipo,
            SaldoInicial = saldoInicial,
            Descricao = descricao,
            Icone = icone,
            Cor = cor ?? "#000000",
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        });
    }

    public Result Atualizar(
        string nome,
        string? descricao = null,
        string? icone = null,
        string? cor = null)
    {
        // Validation: nome não vazio
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(Error.Validation("Nome da conta não pode estar vazio."));

        Nome = nome;
        Descricao = descricao;
        Icone = icone;
        if (cor != null)
            Cor = cor;

        AtualizadoEm = DateTime.UtcNow;
        return Result.Success();
    }

    public void Desativar()
    {
        Ativa = false;
        AtualizadoEm = DateTime.UtcNow;
    }
}
