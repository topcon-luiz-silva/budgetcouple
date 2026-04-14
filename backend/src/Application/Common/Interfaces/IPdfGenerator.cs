namespace BudgetCouple.Application.Common.Interfaces;

using BudgetCouple.Application.Dashboard.DTOs;

public interface IPdfGenerator
{
    byte[] GenerateLancamentosReport(
        List<RelatorioLancamentoDto> lancamentos,
        string fileName = "lancamentos.pdf");

    byte[] GenerateDashboardReport(
        DashboardDto dashboard,
        string fileName = "dashboard.pdf");
}
