namespace BudgetCouple.Application.Dashboard.Queries.ExportDashboard;

using BudgetCouple.Domain.Common;
using MediatR;

public record ExportDashboardPdfQuery(string Mes) : IRequest<Result<byte[]>>;
