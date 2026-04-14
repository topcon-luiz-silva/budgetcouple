namespace BudgetCouple.Domain.Accounting.Lancamentos.ValueObjects;

using BudgetCouple.Domain.Common;

/// <summary>
/// Value object representing money with currency.
/// </summary>
public class Dinheiro : ValueObject
{
    public decimal Valor { get; }
    public string Moeda { get; } // BRL, USD, EUR, etc.

    private Dinheiro(decimal valor, string moeda)
    {
        if (valor < 0)
            throw new ArgumentException("Valor não pode ser negativo.", nameof(valor));

        Valor = valor;
        Moeda = moeda;
    }

    public static Dinheiro Create(decimal valor, string moeda = "BRL")
    {
        return new Dinheiro(valor, moeda);
    }

    public static Dinheiro Zero(string moeda = "BRL")
    {
        return new Dinheiro(0, moeda);
    }

    public Dinheiro Adicionar(Dinheiro outro)
    {
        if (Moeda != outro.Moeda)
            throw new InvalidOperationException("Não é possível adicionar moedas diferentes.");

        return new Dinheiro(Valor + outro.Valor, Moeda);
    }

    public Dinheiro Subtrair(Dinheiro outro)
    {
        if (Moeda != outro.Moeda)
            throw new InvalidOperationException("Não é possível subtrair moedas diferentes.");

        return new Dinheiro(Valor - outro.Valor, Moeda);
    }

    public Dinheiro Multiplicar(decimal fator)
    {
        return new Dinheiro(Valor * fator, Moeda);
    }

    public bool MaiorQue(Dinheiro outro)
    {
        if (Moeda != outro.Moeda)
            throw new InvalidOperationException("Não é possível comparar moedas diferentes.");

        return Valor > outro.Valor;
    }

    public bool MaiorOuIgualA(Dinheiro outro)
    {
        if (Moeda != outro.Moeda)
            throw new InvalidOperationException("Não é possível comparar moedas diferentes.");

        return Valor >= outro.Valor;
    }

    public bool MenorQue(Dinheiro outro)
    {
        if (Moeda != outro.Moeda)
            throw new InvalidOperationException("Não é possível comparar moedas diferentes.");

        return Valor < outro.Valor;
    }

    public bool MenorOuIgualA(Dinheiro outro)
    {
        if (Moeda != outro.Moeda)
            throw new InvalidOperationException("Não é possível comparar moedas diferentes.");

        return Valor <= outro.Valor;
    }

    public static Dinheiro operator +(Dinheiro a, Dinheiro b) => a.Adicionar(b);
    public static Dinheiro operator -(Dinheiro a, Dinheiro b) => a.Subtrair(b);
    public static Dinheiro operator *(Dinheiro a, decimal b) => a.Multiplicar(b);
    public static Dinheiro operator *(decimal a, Dinheiro b) => b.Multiplicar(a);

    public static bool operator >(Dinheiro a, Dinheiro b) => a.MaiorQue(b);
    public static bool operator >=(Dinheiro a, Dinheiro b) => a.MaiorOuIgualA(b);
    public static bool operator <(Dinheiro a, Dinheiro b) => a.MenorQue(b);
    public static bool operator <=(Dinheiro a, Dinheiro b) => a.MenorOuIgualA(b);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
        yield return Moeda;
    }

    public override string ToString()
    {
        return $"{Valor:F2} {Moeda}";
    }
}
