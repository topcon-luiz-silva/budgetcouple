namespace BudgetCouple.Domain.Accounting.Cartoes;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Common;

/// <summary>
/// Aggregate root for credit card.
/// Calculates invoice competence based on closing day.
/// </summary>
public class Cartao : AggregateRoot
{
    public string Nome { get; private set; } = null!;
    public string Bandeira { get; private set; } = null!; // VISA, MASTERCARD, ELO, AMEX
    public int DiaFechamento { get; private set; } // 1-28
    public int DiaVencimento { get; private set; } // 1-28
    public decimal Limite { get; private set; }
    public string? Icone { get; private set; }
    public string Cor { get; private set; } = "#000000";
    public Guid ContaPagamentoId { get; private set; } // Account used for payment
    public bool Ativa { get; private set; } = true;
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    // EF constructor
    protected Cartao() { }

    public static Result<Cartao> Create(
        string nome,
        string bandeira,
        int diaFechamento,
        int diaVencimento,
        decimal limite,
        Guid contaPagamentoId,
        string? icone = null,
        string? cor = null)
    {
        // Validation: nome não vazio
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Cartao>(Error.Validation("Nome do cartão não pode estar vazio."));

        // Validation: diaFechamento 1-28
        if (diaFechamento < 1 || diaFechamento > 28)
            return Result.Failure<Cartao>(Error.Validation("Dia de fechamento deve estar entre 1 e 28."));

        // Validation: diaVencimento 1-28
        if (diaVencimento < 1 || diaVencimento > 28)
            return Result.Failure<Cartao>(Error.Validation("Dia de vencimento deve estar entre 1 e 28."));

        // Validation: limite >= 0
        if (limite < 0)
            return Result.Failure<Cartao>(Error.Validation("Limite não pode ser negativo."));

        return Result.Success(new Cartao
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Bandeira = bandeira,
            DiaFechamento = diaFechamento,
            DiaVencimento = diaVencimento,
            Limite = limite,
            ContaPagamentoId = contaPagamentoId,
            Icone = icone,
            Cor = cor ?? "#000000",
            Ativa = true,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        });
    }

    public Result Atualizar(
        string nome,
        string bandeira,
        int diaFechamento,
        int diaVencimento,
        decimal limite,
        Guid contaPagamentoId,
        string? icone = null,
        string? cor = null)
    {
        // Validation: nome não vazio
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(Error.Validation("Nome do cartão não pode estar vazio."));

        // Validation: diaFechamento 1-28
        if (diaFechamento < 1 || diaFechamento > 28)
            return Result.Failure(Error.Validation("Dia de fechamento deve estar entre 1 e 28."));

        // Validation: diaVencimento 1-28
        if (diaVencimento < 1 || diaVencimento > 28)
            return Result.Failure(Error.Validation("Dia de vencimento deve estar entre 1 e 28."));

        // Validation: limite >= 0
        if (limite < 0)
            return Result.Failure(Error.Validation("Limite não pode ser negativo."));

        Nome = nome;
        Bandeira = bandeira;
        DiaFechamento = diaFechamento;
        DiaVencimento = diaVencimento;
        Limite = limite;
        ContaPagamentoId = contaPagamentoId;
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

    /// <summary>
    /// Calculates the invoice competence (closing month) for a purchase date.
    ///
    /// Rule: Purchase made BEFORE closing day enters current month's invoice.
    ///       Purchase made ON OR AFTER closing day enters NEXT month's invoice.
    ///
    /// Returns: First day (1st) of the competence month.
    /// </summary>
    public DateOnly CalcularCompetenciaFatura(DateOnly dataCompra)
    {
        int dia = dataCompra.Day;
        int mes = dataCompra.Month;
        int ano = dataCompra.Year;

        if (dia < DiaFechamento)
        {
            // Purchase is in current month's invoice
            return new DateOnly(ano, mes, 1);
        }
        else
        {
            // Purchase is in next month's invoice
            // Handle month overflow
            if (mes == 12)
            {
                return new DateOnly(ano + 1, 1, 1);
            }
            else
            {
                return new DateOnly(ano, mes + 1, 1);
            }
        }
    }
}
