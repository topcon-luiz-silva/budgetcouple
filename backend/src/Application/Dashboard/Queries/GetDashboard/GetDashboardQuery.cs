namespace BudgetCouple.Application.Dashboard.Queries.GetDashboard;

using BudgetCouple.Application.Dashboard.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public record GetDashboardQuery(string Mes) : IRequest<Result<DashboardDto>>;
