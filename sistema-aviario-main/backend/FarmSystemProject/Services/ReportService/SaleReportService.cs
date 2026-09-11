using FarmSystemProject.DTOs.Sales;
using FarmSystemProject.Interfaces.IReportService;
using FarmSystemProject.Interfaces.ISales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FarmSystemProject.Services.ReportService;

public class SaleReportService : ISaleReportService
{
    private readonly ISaleService _saleService;

    public SaleReportService(ISaleService saleService)
    {
        _saleService = saleService;
    }

    public async Task<byte[]> GenerateSalesListReport(int ownerId)
    {
        var sales = (await _saleService.GetAllByFarm(ownerId)).ToList();

        return BuildDocument(
            title: "Relatório Geral de Vendas",
            totalLabel: "Valor Total da Granja",
            showDateColumn: true,
            sales: sales);
    }

    public async Task<byte[]> GenerateSalesDateReport(int ownerId, DateTime date)
    {
        var sales = await _saleService.GetAllByFarm(ownerId);
        var dailySales = sales.Where(s => s.SaleDate.Date == date.Date).ToList();

        return BuildDocument(
            title: $"Vendas - {date:dd/MM/yyyy}",
            totalLabel: "Valor Total no Dia",
            showDateColumn: false,
            sales: dailySales);
    }

    // Os dois relatórios só diferem no título, no rodapé e na coluna de data;
    // o resto do desenho é o mesmo.
    private static byte[] BuildDocument(
        string title,
        string totalLabel,
        bool showDateColumn,
        IReadOnlyList<SaleRecordResponse> sales)
    {
        var columnCount = showDateColumn ? 4 : 3;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text(title)
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        if (showDateColumn)
                            columns.ConstantColumn(85);

                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        static IContainer HeaderCell(IContainer cell) =>
                            cell.Background(Colors.Grey.Lighten3).Padding(5);

                        if (showDateColumn)
                            HeaderCell(header.Cell()).Text("Data").SemiBold();

                        HeaderCell(header.Cell()).Text("Val. Uni").SemiBold();
                        HeaderCell(header.Cell()).Text("Qtd").SemiBold();
                        HeaderCell(header.Cell()).Text("Total").SemiBold();
                    });

                    foreach (var item in sales)
                    {
                        var hasNotes = !string.IsNullOrWhiteSpace(item.Notes);

                        // Sem observação a linha fecha com borda; com observação a borda
                        // fica embaixo do texto da observação, mantendo os dois juntos.
                        static IContainer Row(IContainer cell, bool bordered) => bordered
                            ? cell.BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            : cell.PaddingHorizontal(5).PaddingTop(5);

                        if (showDateColumn)
                            Row(table.Cell(), !hasNotes).Text(item.SaleDate.ToString("dd/MM/yyyy"));

                        Row(table.Cell(), !hasNotes).Text($"R$ {item.UnitValue:F2}");
                        Row(table.Cell(), !hasNotes).Text(item.EggQuantity.ToString());
                        Row(table.Cell(), !hasNotes).Text($"R$ {item.TotalValue:F2}");

                        if (hasNotes)
                        {
                            table.Cell().ColumnSpan((uint)columnCount)
                                .BorderBottom(1).BorderColor(Colors.Grey.Lighten4)
                                .PaddingHorizontal(5).PaddingBottom(5)
                                .Text($"Obs.: {item.Notes}")
                                .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                        }
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text($"{totalLabel}: R$ {sales.Sum(s => s.TotalValue):F2}")
                       .SemiBold().FontSize(14);

                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });
        });

        return document.GeneratePdf();
    }
}
