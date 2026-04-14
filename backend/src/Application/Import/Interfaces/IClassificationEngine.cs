namespace BudgetCouple.Application.Import.Interfaces;

/// <summary>
/// Interface for classification engine that applies rules to imported transactions.
/// </summary>
public interface IClassificationEngine
{
    /// <summary>
    /// Classifies a transaction description and returns suggested category and subcategory IDs.
    /// Returns null if no match found.
    /// </summary>
    Task<(Guid? categoriaId, Guid? subcategoriaId)?> ClassifyAsync(string descricao);
}
