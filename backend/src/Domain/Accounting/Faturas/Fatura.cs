namespace BudgetCouple.Domain.Accounting.Faturas;

using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Cartoes;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Accounting.DomainEvents;
using BudgetCouple.Domain.Common;

/// <summary>
/// Calculated entity representing a credit card invoice for a specific month.
/// Not persisted; computed dynamically from Cartao and Lancamentos.
/// Status: ABERTA (open), FECHADA (closed), PAGA (paid).
/// </summary>
public class Fatura
{
    public Cartao Cartao { get; }
    public DateOnly Competencia { get; } // First day of the invoice month
    public List<Lancamento> Lancamentos { get; }
    public decimal Total => Lancamentos.Sum(l => l.Valor);
    public DateOnly DataFechamento { get; }
    public DateOnly DataVencimento { get; }
    public FaturaStatus Status { get; private set; }

    public Fatura(Cartao cartao, DateOnly competencia, IEnumerable<Lancamento> lancamentos)
    {
        Cartao = cartao;
        Competencia = competencia;
        Lancamentos = lancamentos.ToList();

        // Data de fechamento: DiaFechamento no mesmo mês da competência.
        int diaFech = Math.Min(Cartao.DiaFechamento, DateTime.DaysInMonth(Competencia.Year, Competencia.Month));
        DataFechamento = new DateOnly(Competencia.Year, Competencia.Month, diaFech);

        // Calculate due date: DiaVencimento do MESMO mês da competência.
        // Se DiaVencimento < DiaFechamento (vencimento antes do fechamento), avança 1 mês.
        var mesVencimento = Cartao.DiaVencimento < Cartao.DiaFechamento
            ? Competencia.AddMonths(1)
            : Competencia;
        int dia = Math.Min(Cartao.DiaVencimento, DateTime.DaysInMonth(mesVencimento.Year, mesVencimento.Month));
        DataVencimento = new DateOnly(mesVencimento.Year, mesVencimento.Month, dia);

        // Determine status based on payments
        DeterminarStatus();
    }

    private void DeterminarStatus()
    {
        if (Lancamentos.Count == 0)
        {
            Status = FaturaStatus.ABERTA;
            return;
        }

        var todosPagos = Lancamentos.All(l => l.StatusPagamento == StatusPagamento.PAGO);
        var algumPago = Lancamentos.Any(l => l.StatusPagamento == StatusPagamento.PAGO);

        if (todosPagos)
        {
            Status = FaturaStatus.PAGA;
        }
        else if (algumPago)
        {
            Status = FaturaStatus.FECHADA; // Partially paid
        }
        else
        {
            Status = FaturaStatus.ABERTA;
        }
    }

    /// <summary>
    /// Marks all invoice transactions as paid.
    /// Actual payment transaction creation happens in Application layer.
    /// </summary>
    public void MarcarComoPaga(DateOnly dataPagamento, Guid contaDebitoId)
    {
        foreach (var lancamento in Lancamentos)
        {
            lancamento.Pagar(dataPagamento, contaDebitoId);
        }

        DeterminarStatus();
    }

    public bool EstaVencida(DateOnly hoje)
    {
        return hoje > DataVencimento && Status != FaturaStatus.PAGA;
    }
}

public enum FaturaStatus
{
    ABERTA,  // No payments made
    FECHADA, // Partially paid (month has closed)
    PAGA     // Fully paid
}
