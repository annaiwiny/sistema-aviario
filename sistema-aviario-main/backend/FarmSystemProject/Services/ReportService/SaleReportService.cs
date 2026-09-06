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

    public async Task<byte[]> GenerateSalesListReport(int lotId, int ownerId)
    {
        var sales = await _saleService.GetAllByLotId(lotId, ownerId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório Geral de Vendas")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(85);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Val. Uni").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Qtd").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").SemiBold();
                    });

                    foreach (var item in sales)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text(item.SaleDate.ToString("dd/MM/yyyy"));

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"R$ {item.UnitValue:F2}");

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text(item.EggQuantity.ToString());

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"R$ {item.TotalValue:F2}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text($"Valor Total do Lote: R$ {sales.Sum(s => s.TotalValue):F2}")
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

    public async Task<byte[]> GenerateSalesDateReport(int lotId, int ownerId, DateTime date)
    {
        var sales = await _saleService.GetAllByLotId(lotId, ownerId);
        var dailySales = sales.Where(s => s.SaleDate.Date == date.Date).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text($"Vendas - {date:dd/MM/yyyy}")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Val. Uni").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Qtd").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").SemiBold();
                    });

                    foreach (var item in dailySales)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"R$ {item.UnitValue:F2}");

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text(item.EggQuantity.ToString());

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"R$ {item.TotalValue:F2}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span($"Valor Total no Dia: R$ {dailySales.Sum(s => s.TotalValue):F2}");
                    });

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