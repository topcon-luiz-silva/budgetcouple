namespace BudgetCouple.Application.Budgeting.Metas.Commands;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record AtualizarMetaCommand(
    Guid Id,
    string Nome,
    decimal ValorAlvo,
    DateOnly? DataLimite = null,
    int PercentualAlerta = 80) : IRequest<Result<MetaDto>>;
