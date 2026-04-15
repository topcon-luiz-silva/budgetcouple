namespace BudgetCouple.Infrastructure.Services.Reports;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Dashboard.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class PdfGenerator : IPdfGenerator
{
    public byte[] GenerateLancamentosReport(
        List<RelatorioLancamentoDto> lancamentos,
        string fileName = "lancamentos.pdf")
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

                // Header
                page.Header().Column(col =>
                {
                    col.Item().Text("Relatório de Lançamentos").FontSize(20).Bold();
                    col.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").FontSize(10);
                });

                // Content
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1); // Descrição
                        columns.RelativeColumn(1); // Valor
                        columns.RelativeColumn(0.8f); // Data
                        columns.RelativeColumn(0.8f); // Tipo
                        columns.RelativeColumn(0.8f); // Natureza
                        columns.RelativeColumn(0.8f); // Status
                        columns.RelativeColumn(1); // Categoria
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background("#2C3E50").Text("Descrição").FontColor("white").Bold();
                        header.Cell().Background("#2C3E50").Text("Valor").FontColor("white").Bold().AlignRight();
                        header.Cell().Background("#2C3E50").Text("Data").FontColor("white").Bold();
                        header.Cell().Background("#2C3E50").Text("Tipo").FontColor("white").Bold();
                        header.Cell().Background("#2C3E50").Text("Natureza").FontColor("white").Bold();
                        header.Cell().Background("#2C3E50").Text("Status").FontColor("white").Bold();
                        header.Cell().Background("#2C3E50").Text("Categoria").FontColor("white").Bold();
                    });

                    // Rows
                    foreach (var lancamento in lancamentos)
                    {
                        table.Cell().Text(lancamento.Descricao);
                        table.Cell().Text($"R$ {lancamento.Valor:N2}").AlignRight();
                        table.Cell().Text(lancamento.Data.ToString("dd/MM/yyyy"));
                        table.Cell().Text(lancamento.Tipo);
                        table.Cell().Text(lancamento.Natureza);
                        table.Cell().Text(lancamento.Status);
                        table.Cell().Text(lancamento.CategoriaNome ?? "");
                    }
                });

                // Footer
                page.Footer()
                    .AlignCenter()
                    .Text($"Gerado em {DateTime.Now:dd/MM/yyyy}")
                    .FontSize(10);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateDashboardReport(
        DashboardDto dashboard,
        string fileName = "dashboard.pdf")
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

                // Header
                page.Header().Column(col =>
                {
                    col.Item().Text($"BudgetCouple - Dashboard {dashboard.Competencia}")
                        .FontSize(20).Bold();
                    col.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .FontSize(10);
                });

                page.Content().Column(col =>
                {
                    // Resumo Section
                    col.Item().Text("Resumo Financeiro").FontSize(14).Bold();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        AddResumoRow(table, "Total Receitas", dashboard.Resumo.TotalReceitas);
                        AddResumoRow(table, "Total Despesas", dashboard.Resumo.TotalDespesas);
                        AddResumoRow(table, "Saldo", dashboard.Resumo.Saldo);
                        AddResumoRow(table, "Total Previsto Receitas", dashboard.Resumo.TotalPrevistoReceitas);
                        AddResumoRow(table, "Total Previsto Despesas", dashboard.Resumo.TotalPrevistoDespesas);
                        AddResumoRow(table, "Saldo Consolidado Contas", dashboard.Resumo.SaldoConsolidadoContas);
                    });

                    // Por Categoria
                    if (dashboard.PorCategoria.Count > 0)
                    {
                        col.Item().Text("Despesas por Categoria").FontSize(14).Bold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(0.8f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#2C3E50").Text("Categoria").FontColor("white").Bold();
                                header.Cell().Background("#2C3E50").Text("Total").FontColor("white").Bold();
                                header.Cell().Background("#2C3E50").Text("% Total").FontColor("white").Bold();
                            });

                            foreach (var cat in dashboard.PorCategoria.Take(10))
                            {
                                table.Cell().Text(cat.CategoriaNome);
                                table.Cell().Text($"R$ {cat.TotalDespesas:N2}");
                                table.Cell().Text($"{cat.Percentual:N1}%");
                            }
                        });
                    }

                    // Por Conta
                    if (dashboard.PorConta.Count > 0)
                    {
                        col.Item().Text("Saldo por Conta").FontSize(14).Bold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#2C3E50").Text("Conta").FontColor("white").Bold();
                                header.Cell().Background("#2C3E50").Text("Saldo").FontColor("white").Bold();
                            });

                            foreach (var conta in dashboard.PorConta)
                            {
                                table.Cell().Text(conta.ContaNome);
                                table.Cell().Text($"R$ {conta.Saldo:N2}");
                            }
                        });
                    }
                });

                // Footer
                page.Footer()
                    .AlignCenter()
                    .Text($"Gerado em {DateTime.Now:dd/MM/yyyy}")
                    .FontSize(10);
            });
        });

        return document.GeneratePdf();
    }

    private void AddResumoRow(TableDescriptor table, string label, decimal value)
    {
        table.Cell().Text(label);
        table.Cell().Text($"R$ {value:N2}").AlignRight();
    }
}
