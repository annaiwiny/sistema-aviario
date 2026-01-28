using FarmSystemProject.Interfaces;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using Microsoft.Identity.Client;
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
    public async Task<byte[]> GenerateVaccinationListReport()
    {
        var vaccination = await _vaccinationService.GetAll();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório de Vacinação")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Tipo da vacina").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Valor unitário").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Lote").SemiBold();
                    });

                    foreach (var vaccination in vaccination)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.Id.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.ApplicationDate.ToString("dd/MM/yyyy"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.VaccineType);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.ApplicationValue.ToString("F2"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.ApplicationQuantity.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.LotId.ToString());

                    }

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateVaccinationDateReport(DateTime applicationDate)
    {
        var vaccination = await _vaccinationService.GetByDate(applicationDate);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório de Vacinação")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Tipo da vacina").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Valor unitário").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Lote").SemiBold();

                        foreach (var vaccination in vaccination)
                        {
                            if (vaccination.ApplicationDate.Date == applicationDate.Date)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.Id.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.ApplicationDate.ToString("dd/MM/yyyy"));
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.VaccineType);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.ApplicationValue.ToString("F2"));
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.ApplicationQuantity.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(vaccination.LotId.ToString());
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
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