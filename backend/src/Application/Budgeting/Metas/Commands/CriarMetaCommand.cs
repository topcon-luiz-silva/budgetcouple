namespace BudgetCouple.Application.Budgeting.Metas.Commands;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// Create a new Meta (ECONOMIA or REDUCAO_CATEGORIA).
/// Type-specific fields are conditionally present based on "Tipo" value.
/// </summary>
public record CriarMetaCommand(
    string Tipo, // "ECONOMIA" | "REDUCAO_CATEGORIA"
    string Nome,
    decimal ValorAlvo,
    DateOnly? DataLimite = null,
    Guid? CategoriaId = null,
    string? Periodo = null,
    int PercentualAlerta = 80) : IRequest<Result<MetaDto>>;
