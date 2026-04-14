namespace BudgetCouple.Application.Import.DTOs;

/// <summary>
/// Response from import preview endpoint.
/// </summary>
public class ImportPreviewDto
{
    public List<ImportItemDto> Lancamentos { get; set; } = new();
    public int TotalItens => Lancamentos.Count;
    public int TotalDuplicados => Lancamentos.Count(x => x.Duplicado);
    public int TotalNovos => TotalItens - TotalDuplicados;
}
