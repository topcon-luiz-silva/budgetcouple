namespace BudgetCouple.Infrastructure.Services.Reports;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Dashboard.DTOs;
using ClosedXML.Excel;
using System.Globalization;

public class ExcelGenerator : IExcelGenerator
{
    public byte[] GenerateLancamentosReport(
        List<RelatorioLancamentoDto> lancamentos,
        string fileName = "lancamentos.xlsx")
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Lançamentos");

            // Header
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.DarkGray;
            headerRow.Style.Font.FontColor = XLColor.White;

            var headers = new[] { "ID", "Descrição", "Valor", "Data", "Tipo", "Natureza", "Status", "Categoria", "Conta", "Cartão", "Data Pagamento" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Data rows
            int rowNum = 2;
            foreach (var lancamento in lancamentos)
            {
                worksheet.Cell(rowNum, 1).Value = lancamento.Id.ToString().Substring(0, 8);
                worksheet.Cell(rowNum, 2).Value = lancamento.Descricao;
                worksheet.Cell(rowNum, 3).Value = lancamento.Valor;
                worksheet.Cell(rowNum, 4).Value = lancamento.Data.ToString("yyyy-MM-dd");
                worksheet.Cell(rowNum, 5).Value = lancamento.Tipo;
                worksheet.Cell(rowNum, 6).Value = lancamento.Natureza;
                worksheet.Cell(rowNum, 7).Value = lancamento.Status;
                worksheet.Cell(rowNum, 8).Value = lancamento.CategoriaNome ?? "";
                worksheet.Cell(rowNum, 9).Value = lancamento.ContaNome ?? "";
                worksheet.Cell(rowNum, 10).Value = lancamento.CartaoNome ?? "";
                worksheet.Cell(rowNum, 11).Value = lancamento.DataPagamento?.ToString("yyyy-MM-dd") ?? "";

                // Format money columns
                worksheet.Cell(rowNum, 3).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

                rowNum++;
            }

            // Autofit columns
            worksheet.Columns().AdjustToContents();

            // Convert to bytes
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

    public byte[] GenerateDashboardReport(
        DashboardDto dashboard,
        string fileName = "dashboard.xlsx")
    {
        using (var workbook = new XLWorkbook())
        {
            // Resumo
            var resumoSheet = workbook.Worksheets.Add("Resumo");
            resumoSheet.Cell("A1").Value = "Competência";
            resumoSheet.Cell("B1").Value = dashboard.Competencia;

            resumoSheet.Cell("A2").Value = "Total Receitas";
            resumoSheet.Cell("B2").Value = dashboard.Resumo.TotalReceitas;
            resumoSheet.Cell("B2").Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

            resumoSheet.Cell("A3").Value = "Total Despesas";
            resumoSheet.Cell("B3").Value = dashboard.Resumo.TotalDespesas;
            resumoSheet.Cell("B3").Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

            resumoSheet.Cell("A4").Value = "Saldo";
            resumoSheet.Cell("B4").Value = dashboard.Resumo.Saldo;
            resumoSheet.Cell("B4").Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

            resumoSheet.Cell("A5").Value = "Total Previsto Receitas";
            resumoSheet.Cell("B5").Value = dashboard.Resumo.TotalPrevistoReceitas;
            resumoSheet.Cell("B5").Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

            resumoSheet.Cell("A6").Value = "Total Previsto Despesas";
            resumoSheet.Cell("B6").Value = dashboard.Resumo.TotalPrevistoDespesas;
            resumoSheet.Cell("B6").Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

            resumoSheet.Cell("A7").Value = "Saldo Consolidado Contas";
            resumoSheet.Cell("B7").Value = dashboard.Resumo.SaldoConsolidadoContas;
            resumoSheet.Cell("B7").Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

            // Por Categoria
            if (dashboard.PorCategoria.Count > 0)
            {
                var categoriaSheet = workbook.Worksheets.Add("Por Categoria");
                AddCategorySheet(categoriaSheet, dashboard.PorCategoria);
            }

            // Por Conta
            if (dashboard.PorConta.Count > 0)
            {
                var contaSheet = workbook.Worksheets.Add("Por Conta");
                AddAccountSheet(contaSheet, dashboard.PorConta);
            }

            // Por Cartão
            if (dashboard.PorCartao.Count > 0)
            {
                var cartaoSheet = workbook.Worksheets.Add("Por Cartão");
                AddCardSheet(cartaoSheet, dashboard.PorCartao);
            }

            // Evolução Mensal
            if (dashboard.EvolucaoMensal.Count > 0)
            {
                var evolucaoSheet = workbook.Worksheets.Add("Evolução Mensal");
                AddEvolutionSheet(evolucaoSheet, dashboard.EvolucaoMensal);
            }

            // Convert to bytes
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

    private void AddCategorySheet(IXLWorksheet worksheet, List<CategoriaResumoDto> categorias)
    {
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.DarkGray;
        headerRow.Style.Font.FontColor = XLColor.White;

        worksheet.Cell("A1").Value = "Categoria";
        worksheet.Cell("B1").Value = "Total Despesas";
        worksheet.Cell("C1").Value = "Total Receitas";
        worksheet.Cell("D1").Value = "Percentual";

        int rowNum = 2;
        foreach (var cat in categorias)
        {
            worksheet.Cell(rowNum, 1).Value = cat.CategoriaNome;
            worksheet.Cell(rowNum, 2).Value = cat.TotalDespesas;
            worksheet.Cell(rowNum, 3).Value = cat.TotalReceitas;
            worksheet.Cell(rowNum, 4).Value = cat.Percentual;

            worksheet.Cell(rowNum, 2).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";
            worksheet.Cell(rowNum, 3).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";
            worksheet.Cell(rowNum, 4).Style.NumberFormat.Format = "0.00%";

            rowNum++;
        }

        worksheet.Columns().AdjustToContents();
    }

    private void AddAccountSheet(IXLWorksheet worksheet, List<ContaResumoDto> contas)
    {
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.DarkGray;
        headerRow.Style.Font.FontColor = XLColor.White;

        worksheet.Cell("A1").Value = "Conta";
        worksheet.Cell("B1").Value = "Saldo";
        worksheet.Cell("C1").Value = "Total Entradas";
        worksheet.Cell("D1").Value = "Total Saídas";

        int rowNum = 2;
        foreach (var conta in contas)
        {
            worksheet.Cell(rowNum, 1).Value = conta.ContaNome;
            worksheet.Cell(rowNum, 2).Value = conta.Saldo;
            worksheet.Cell(rowNum, 3).Value = conta.TotalEntradas;
            worksheet.Cell(rowNum, 4).Value = conta.TotalSaidas;

            worksheet.Cell(rowNum, 2).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";
            worksheet.Cell(rowNum, 3).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";
            worksheet.Cell(rowNum, 4).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

            rowNum++;
        }

        worksheet.Columns().AdjustToContents();
    }

    private void AddCardSheet(IXLWorksheet worksheet, List<CartaoResumoDto> cartoes)
    {
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.DarkGray;
        headerRow.Style.Font.FontColor = XLColor.White;

        worksheet.Cell("A1").Value = "Cartão";
        worksheet.Cell("B1").Value = "Valor Fatura";
        worksheet.Cell("C1").Value = "Limite";
        worksheet.Cell("D1").Value = "% Utilizado";

        int rowNum = 2;
        foreach (var cartao in cartoes)
        {
            worksheet.Cell(rowNum, 1).Value = cartao.CartaoNome;
            worksheet.Cell(rowNum, 2).Value = cartao.ValorFatura;
            worksheet.Cell(rowNum, 3).Value = cartao.Limite;
            worksheet.Cell(rowNum, 4).Value = cartao.LimiteUtilizadoPct;

            worksheet.Cell(rowNum, 2).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";
            worksheet.Cell(rowNum, 3).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";
            worksheet.Cell(rowNum, 4).Style.NumberFormat.Format = "0.00%";

            rowNum++;
        }

        worksheet.Columns().AdjustToContents();
    }

    private void AddEvolutionSheet(IXLWorksheet worksheet, List<EvolucaoMesDto> evolucao)
    {
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.DarkGray;
        headerRow.Style.Font.FontColor = XLColor.White;

        worksheet.Cell("A1").Value = "Competência";
        worksheet.Cell("B1").Value = "Receitas";
        worksheet.Cell("C1").Value = "Despesas";
        worksheet.Cell("D1").Value = "Saldo";

        int rowNum = 2;
        foreach (var mes in evolucao)
        {
            worksheet.Cell(rowNum, 1).Value = mes.Competencia;
            worksheet.Cell(rowNum, 2).Value = mes.Receitas;
            worksheet.Cell(rowNum, 3).Value = mes.Despesas;
            worksheet.Cell(rowNum, 4).Value = mes.Saldo;

            worksheet.Cell(rowNum, 2).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";
            worksheet.Cell(rowNum, 3).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";
            worksheet.Cell(rowNum, 4).Style.NumberFormat.Format = "_-[$R$-pt-BR] * #,##0.00_-;-[$R$-pt-BR] * #,##0.00_-;_-[$R$-pt-BR] * \"-\"??_-;_-@_-";

            rowNum++;
        }

        worksheet.Columns().AdjustToContents();
    }
}
