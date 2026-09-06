using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Interfaces.IReportService;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FarmSystemProject.Services.ReportService;

public class VaccinationReportService : IVaccinationReportService
{
    private readonly IVaccinationService _vaccinationService;

    public VaccinationReportService(IVaccinationService vaccinationService)
    {
        _vaccinationService = vaccinationService;
    }

    public async Task<byte[]> GenerateVaccinationListReport(int lotId, int ownerId)
    {
        var vaccinations = await _vaccinationService.GetAllByLotId(lotId, ownerId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório Geral de Vacinação")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.ConstantColumn(85);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Vacina").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Val. Uni").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Qtd").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").SemiBold();
                    });

                    foreach (var item in vaccinations)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.Id.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.ApplicationDate.ToString("dd/MM/yyyy"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.VaccineType);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text($"R$ {item.ApplicationValue:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.ApplicationQuantity.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text($"R$ {item.TotalCost:F2}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text($"Custo Total do Lote: R$ {vaccinations.Sum(v => v.TotalCost):F2}")
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

    public async Task<byte[]> GenerateVaccinationDateReport(int lotId, int ownerId, DateTime date)
    {
        var allVaccinations = await _vaccinationService.GetAllByLotId(lotId, ownerId);
        var dailyVaccinations = allVaccinations.Where(v => v.ApplicationDate.Date == date.Date).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text($"Vacinação - {date:dd/MM/yyyy}")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Vacina").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Val. Uni").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Qtd").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").SemiBold();
                    });

                    foreach (var item in dailyVaccinations)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.Id.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.VaccineType);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text($"R$ {item.ApplicationValue:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(item.ApplicationQuantity.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text($"R$ {item.TotalCost:F2}");
                    }
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span($"Total Gasto no Dia: R$ {dailyVaccinations.Sum(v => v.TotalCost):F2}");
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