using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using FarmSystemProject.Interfaces.IReportInterface;
using FarmSystemProject.Interfaces.IProductiveMonitoring;

namespace FarmSystemProject.Services.ReportService;

public class EggProductionReportService : IEggProductionReportService
{
    private readonly IEggProductionService _eggService;

    public EggProductionReportService(IEggProductionService eggService)
    {
        _eggService = eggService;
    }

    public async Task<byte[]> GenerateEggListReport(int lotId, int ownerId)
    {
        var productions = await _eggService.GetAllByLotId(lotId, ownerId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório de Produção de Ovos")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(85);
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade").SemiBold();
                    });

                    foreach (var item in productions)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.Id.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.ProductionDate.ToString("dd/MM/yyyy"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.Quantity.ToString());
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateEggDateReport(int lotId, int ownerId, DateTime date)
    {
        var allData = await _eggService.GetAllByLotId(lotId, ownerId);
        var dailyData = allData.Where(p => p.ProductionDate.Date == date.Date).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório de Produção de Ovos")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(85);
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade").SemiBold();
                    });

                    foreach (var item in dailyData)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.Id.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.ProductionDate.ToString("dd/MM/yyyy"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.Quantity.ToString());
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        });

        return document.GeneratePdf();
    }
}