namespace BudgetCouple.Application.Import.DTOs;

/// <summary>
/// Request to confirm import and create transactions.
/// </summary>
public class ConfirmImportDto
{
    public Guid? ContaId { get; set; }
    public Guid? CartaoId { get; set; }
    public List<LancamentoImportacaoDto> Lancamentos { get; set; } = new();
}

public class LancamentoImportacaoDto
{
    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateOnly DataCompetencia { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid? SubcategoriaId { get; set; }
    public string HashImportacao { get; set; } = null!;
}

/// <summary>
/// Response from confirm import.
/// </summary>
public class ConfirmImportResultDto
{
    public int Sucesso { get; set; }
    public int Falhas { get; set; }
    public List<string> Mensagens { get; set; } = new();
}
