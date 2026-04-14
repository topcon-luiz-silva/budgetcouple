namespace BudgetCouple.Application.Common.Interfaces;

using BudgetCouple.Application.Dashboard.DTOs;

public interface IExcelGenerator
{
    byte[] GenerateLancamentosReport(
        List<RelatorioLancamentoDto> lancamentos,
        string fileName = "lancamentos.xlsx");

    byte[] GenerateDashboardReport(
        DashboardDto dashboard,
        string fileName = "dashboard.xlsx");
}
