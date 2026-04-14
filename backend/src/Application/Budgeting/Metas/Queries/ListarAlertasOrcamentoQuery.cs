namespace BudgetCouple.Application.Budgeting.Metas.Queries;

using BudgetCouple.Application.Budgeting.Metas.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

/// <summary>
/// List all budget alerts for the current month.
/// Includes REDUCAO_CATEGORIA metas where current month spending >= 80% of target.
/// </summary>
public record ListarAlertasOrcamentoQuery : IRequest<Result<List<AlertaOrcamentoDashboardDto>>>;
