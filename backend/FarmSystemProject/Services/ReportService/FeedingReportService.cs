using FarmSystemProject.Interfaces.INutritionalControl;
using FarmSystemProject.Interfaces.IReportService;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FarmSystemProject.Services.ReportService;

public class FeedingReportService : IFeedingReportService
{
    private readonly IFeedingService _feedingService;

    public FeedingReportService(IFeedingService feedingService)
    {
        _feedingService = feedingService;
    }

    public async Task<byte[]> GenerateFeedingListReport(int lotId, int ownerId)
    {
        var feedings = await _feedingService.GetAllByLotId(lotId, ownerId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório Geral de Controle de Alimentação")
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
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade consumida (kg)").SemiBold();
                    });

                    foreach (var item in feedings)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text(item.ConsumptionDate.ToString("dd/MM/yyyy"));

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"{item.ConsumptionQuantity:F2}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text($"Quantidade Total Consumida do Lote: {feedings.Sum(s => s.ConsumptionQuantity):F2} kg")
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

    public async Task<byte[]> GenerateFeedingDateReport(int lotId, int ownerId, DateTime date)
    {
        var feedings = await _feedingService.GetAllByLotId(lotId, ownerId);
        var dailyFeedings = feedings.Where(f => f.ConsumptionDate.Date == date.Date).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text($"Controle de Alimentação - {date:dd/MM/yyyy}")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade de ração consumida (kg)").SemiBold();
                    });

                    foreach (var item in dailyFeedings)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"{item.ConsumptionQuantity:F2}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span($"Quantidade Total Consumida no Dia: {dailyFeedings.Sum(f => f.ConsumptionQuantity):F2} kg");
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