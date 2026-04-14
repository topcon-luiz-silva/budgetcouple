namespace BudgetCouple.Application.Budgeting.Metas.DTOs;

/// <summary>
/// DTO for Meta (both ECONOMIA and REDUCAO_CATEGORIA).
/// Type discriminator determines which subclass it represents.
/// </summary>
public record MetaDto(
    Guid Id,
    string Tipo, // "ECONOMIA" | "REDUCAO_CATEGORIA"
    string Nome,
    decimal ValorAlvo,
    decimal ValorAtual,
    DateOnly? DataLimite,
    Guid? CategoriaId,
    string? Periodo, // MENSAL, TRIMESTRAL, ANUAL
    int PercentualAlerta,
    bool Ativa,
    DateTime CriadoEm,
    DateTime AtualizadoEm);

/// <summary>
/// Progress information for a Meta.
/// Contents vary based on Meta type.
/// </summary>
public record MetaProgressoDto(
    Guid MetaId,
    string Tipo,
    decimal ValorAtual,
    decimal ValorAlvo,
    decimal PercentualProgresso,
    int? DiasRestantes, // For ECONOMIA
    bool AtingiuAlerta,
    bool Atingida);

/// <summary>
/// DTO for listing budget alerts in Dashboard.
/// </summary>
public record AlertaOrcamentoDashboardDto(
    Guid MetaId,
    string NomeMeta,
    string? CategoriaNome,
    decimal ValorAtual,
    decimal ValorAlvo,
    decimal PercentualUtilizado);
