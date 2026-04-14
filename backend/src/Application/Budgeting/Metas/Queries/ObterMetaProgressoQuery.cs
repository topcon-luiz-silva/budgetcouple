namespace BudgetCouple.Application.Budgeting.Metas.Queries;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// Get current progress of a Meta, including calculated fields like dias restantes, percentual, etc.
/// </summary>
public record ObterMetaProgressoQuery(Guid MetaId) : IRequest<Result<MetaProgressoDto>>;
