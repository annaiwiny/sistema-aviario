using FarmSystemProject.Interfaces.INutritionalControl;
using FarmSystemProject.Interfaces.IReportService;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FarmSystemProject.Services.ReportService;

public class FeedReportService : IFeedReportService
{
    private readonly IFeedService _feedService;

    public FeedReportService(IFeedService feedService)
    {
        _feedService = feedService;
    }

    public async Task<byte[]> GenerateFeedListReport(int lotId, int ownerId)
    {
        var feeds = await _feedService.GetAllByLotId(lotId, ownerId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório Geral de Gastos com Ração")
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
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Peso").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Qtd").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Val. Uni").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").SemiBold();
                    });

                    foreach (var item in feeds)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text(item.PurchaseDate.ToString("dd/MM/yyyy"));

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"{item.BagWeight:F2} kg");

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text(item.BagQuantity.ToString());

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"R$ {item.BagValue:F2}");

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"R$ {item.TotalCost:F2}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text($"Custo Total do Lote: R$ {feeds.Sum(f => f.TotalCost):F2}")
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

    public async Task<byte[]> GenerateFeedDateReport(int lotId, int ownerId, DateTime date)
    {
        var feeds = await _feedService.GetAllByLotId(lotId, ownerId);
        var dailyFeeds = feeds.Where(f => f.PurchaseDate.Date == date.Date).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text($"Gastos com Ração - {date:dd/MM/yyyy}")
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
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Peso").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Qtd").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Val. Uni").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").SemiBold();
                    });

                    foreach (var item in dailyFeeds)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"{item.BagWeight:F2} kg");

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text(item.BagQuantity.ToString());

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"R$ {item.BagValue:F2}");

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5)
                            .Text($"R$ {item.TotalCost:F2}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span($"Custo Total no Dia: R$ {dailyFeeds.Sum(f => f.TotalCost):F2}");
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