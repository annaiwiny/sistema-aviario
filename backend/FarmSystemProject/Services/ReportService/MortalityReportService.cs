using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Interfaces.IFarm;
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
        // Busca as raças usando seu serviço existente
        var mortality = await _mortalityService.GetAll();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Cabeçalho
                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("Relatório de Mortalidade")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                        .FontSize(10).Italic();
                });

                // Conteúdo principal (Tabela)
                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50); // Coluna para ID
                        columns.RelativeColumn();    // Coluna para Nome da Raça
                    });

                    // Cabeçalho da Tabela
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ID").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Nome da Raça").SemiBold();
                    });

                    // Linhas da Tabela
                    foreach (var mortality in mortality)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.Id.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.DateDeath.ToString("dd/MM/yyyy"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.DeathQuantity.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.CutQuantity.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.Reason);   
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(mortality.LotId.ToString());
                    }
                });

                // Rodapé
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