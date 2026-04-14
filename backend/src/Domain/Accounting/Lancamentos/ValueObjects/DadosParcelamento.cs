namespace BudgetCouple.Domain.Accounting.Lancamentos.ValueObjects;

using BudgetCouple.Domain.Common;

/// <summary>
/// Value object representing installment data for a transaction.
/// Valid only when the transaction is marked as parcelada (IsParcelada = true).
/// </summary>
public class DadosParcelamento : ValueObject
{
    public int ParcelaAtual { get; }
    public int TotalParcelas { get; }
    public Guid? LancamentoPaiId { get; }

    private DadosParcelamento(int parcelaAtual, int totalParcelas, Guid? lancamentoPaiId = null)
    {
        if (parcelaAtual < 1)
            throw new ArgumentException("ParcelaAtual deve ser >= 1.", nameof(parcelaAtual));

        if (totalParcelas < 2)
            throw new ArgumentException("TotalParcelas deve ser >= 2.", nameof(totalParcelas));

        if (parcelaAtual > totalParcelas)
            throw new ArgumentException("ParcelaAtual não pode ser maior que TotalParcelas.", nameof(parcelaAtual));

        ParcelaAtual = parcelaAtual;
        TotalParcelas = totalParcelas;
        LancamentoPaiId = lancamentoPaiId;
    }

    /// <summary>
    /// Creates installment data for the parent transaction (first installment).
    /// </summary>
    public static DadosParcelamento CriarPai(int totalParcelas)
    {
        return new DadosParcelamento(1, totalParcelas, null);
    }

    /// <summary>
    /// Creates installment data for a child transaction.
    /// </summary>
    public static DadosParcelamento CriarFilho(int parcelaAtual, int totalParcelas, Guid lancamentoPaiId)
    {
        if (lancamentoPaiId == Guid.Empty)
            throw new ArgumentException("LancamentoPaiId não pode ser vazio.", nameof(lancamentoPaiId));

        return new DadosParcelamento(parcelaAtual, totalParcelas, lancamentoPaiId);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ParcelaAtual;
        yield return TotalParcelas;
        yield return LancamentoPaiId;
    }

    public override string ToString()
    {
        return $"Parcela {ParcelaAtual}/{TotalParcelas}";
    }
}
