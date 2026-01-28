using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Interfaces.IHealthMonitoring;

namespace FarmSystemProject.Services.ReportService;

public class MortalityReportService : IMortalityReportService
{
    private readonly IMortalityService _mortalityService;

    public MortalityReportService(IMortalityService mortalityService)
    {
        _mortalityService = mortalityService;
    }

    public async Task<byte[]> GenerateMortalityListReport()
    {
        var mortality = await _mortalityService.GetAll();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório de Mortalidade")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(50);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.ConstantColumn(50);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade da Morte").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade de corte").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Motivo").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Lote").SemiBold();
                    });

                    foreach (var mortality in mortality)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.Id.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.DateDeath.ToString("dd/MM/yyyy"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.DeathQuantity.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.CutQuantity.ToString());
<<<<<<< Updated upstream
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.Reason);   
=======
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.Reason);
>>>>>>> Stashed changes
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.LotId.ToString());
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
    public async Task<byte[]> GenerateMortalityDateReport(DateTime dateDeath)
    {
        var mortality = await _mortalityService.GetByDate(dateDeath);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório de Mortalidade")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
<<<<<<< Updated upstream
                        columns.ConstantColumn(50); 
                        columns.ConstantColumn(50);   
=======
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(50);
>>>>>>> Stashed changes
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.ConstantColumn(50);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Data").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade da Morte").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantidade de corte").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Motivo").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Lote").SemiBold();
                    });

                    foreach (var mortality in mortality)
                    {
<<<<<<< Updated upstream
                        if(mortality.DateDeath.Date == dateDeath)
=======
                        if (mortality.DateDeath.Date == dateDeath)
>>>>>>> Stashed changes
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.Id.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.DateDeath.ToString("dd/MM/yyyy"));
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.DeathQuantity.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.CutQuantity.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.Reason);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.LotId.ToString());
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

        return document.GeneratePdf();
    }

}