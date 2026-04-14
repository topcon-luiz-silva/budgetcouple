namespace BudgetCouple.Application.Import.DTOs;

/// <summary>
/// Represents a single transaction item from import preview.
/// </summary>
public class ImportItemDto
{
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateOnly DataCompetencia { get; set; }
    public Guid? CategoriaSugeridaId { get; set; }
    public string? CategoriaSugeridaNome { get; set; }
    public bool Duplicado { get; set; }
    public string HashImportacao { get; set; } = null!;
}
