namespace BudgetCouple.Application.Dashboard.Queries.ExportDashboard;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Dashboard.Queries.GetDashboard;
using BudgetCouple.Domain.Common;
using MediatR;

public class ExportDashboardPdfQueryHandler : IRequestHandler<ExportDashboardPdfQuery, Result<byte[]>>
{
    private readonly IMediator _mediator;
    private readonly IPdfGenerator _pdfGenerator;

    public ExportDashboardPdfQueryHandler(
        IMediator mediator,
        IPdfGenerator pdfGenerator)
    {
        _mediator = mediator;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<Result<byte[]>> Handle(ExportDashboardPdfQuery request, CancellationToken cancellationToken)
    {
        // Get dashboard data
        var dashboardResult = await _mediator.Send(new GetDashboardQuery(request.Mes), cancellationToken);

        if (dashboardResult.IsFailure)
            return Result.Failure<byte[]>(dashboardResult.Error);

        // Generate PDF
        var bytes = _pdfGenerator.GenerateDashboardReport(dashboardResult.Value, $"dashboard-{request.Mes}.pdf");

        return Result.Success(bytes);
    }
}
