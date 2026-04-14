namespace BudgetCouple.Application.Import.Interfaces;

/// <summary>
/// Represents a parsed OFX transaction.
/// </summary>
public class OfxTransacao
{
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateOnly Data { get; set; }
    public string TransacaoId { get; set; } = null!;
}

/// <summary>
/// Interface for parsing OFX files.
/// </summary>
public interface IOfxParser
{
    /// <summary>
    /// Parses an OFX file and extracts transactions.
    /// Supports OFX 1.x (SGML) and 2.x (XML).
    /// </summary>
    Task<List<OfxTransacao>> ParseAsync(Stream stream);
}
