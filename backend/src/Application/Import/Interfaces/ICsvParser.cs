namespace BudgetCouple.Application.Import.Interfaces;

/// <summary>
/// Represents a parsed CSV transaction.
/// </summary>
public class CsvTransacao
{
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateOnly Data { get; set; }
}

/// <summary>
/// Interface for parsing CSV files.
/// </summary>
public interface ICsvParser
{
    /// <summary>
    /// Parses a CSV file and extracts transactions.
    /// Supports flexible headers (Data, Descrição, Valor variations).
    /// </summary>
    Task<List<CsvTransacao>> ParseAsync(Stream stream);
}
