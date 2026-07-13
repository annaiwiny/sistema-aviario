using FarmSystemProject.Interfaces.IReportService;
using FarmSystemProject.Interfaces.ISensors;
using FarmSystemProject.Models.Sensors;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.IO;

namespace FarmSystemProject.Services.ReportService;

public class SensorReportService : ISensorReportService
{
    private readonly ISensorService _sensorService;

    public SensorReportService(ISensorService sensorService)
    {
        _sensorService = sensorService;
    }

    public async Task<byte[]> GenerateSensorMonitoringReport(int lotId, int ownerId, SensorType type)
    {
        var now = DateTime.Now;

        var lastHourReadings = await _sensorService.GetReadingsHistory(lotId, ownerId, type, now.AddHours(-1));
        var last24hReadings = await _sensorService.GetReadingsHistory(lotId, ownerId, type, now.AddHours(-24));
        var last7dReadings = await _sensorService.GetReadingsHistory(lotId, ownerId, type, now.AddDays(-7));

        return BuildReportFromReadings(type, lastHourReadings, last24hReadings, last7dReadings);
    }

    public byte[] BuildReportFromReadings(
        SensorType type,
        List<SensorReading> lastHourReadings,
        List<SensorReading> last24hReadings,
        List<SensorReading> last7dReadings)
    {
        var now = DateTime.Now;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Text($"Relatório de Monitoramento de {TranslateType(type)}")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    row.RelativeItem().AlignRight().Text(now.ToString("dd/MM/yyyy HH:mm"))
                        .FontSize(10).Italic();
                });

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(12);

                    column.Item().Element(c => BuildSection(c, "ÚLTIMA HORA", lastHourReadings, type));
                    column.Item().Element(c => BuildSection(c, "ÚLTIMAS 24 HORAS", last24hReadings, type));
                    column.Item().Element(c => BuildSection(c, "ÚLTIMOS 7 DIAS", last7dReadings, type));
                });

                page.Footer().AlignCenter().Text("Relatório gerado automaticamente pelo sistema")
                    .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
            });
        });

        return document.GeneratePdf();
    }

    private void BuildSection(IContainer container, string title, List<SensorReading> readings, SensorType type)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Row(row =>
        {
            row.RelativeItem(2).Column(col =>
            {
                col.Item().Text(title).SemiBold().FontSize(14);
                col.Item().PaddingTop(5).Height(150).Image(RenderChartImage(readings)).FitArea();
            });

            row.ConstantItem(15);

            row.RelativeItem(1).Element(c => BuildInfoCard(c, readings, type));
        });
    }
    private byte[] RenderChartImage(List<SensorReading> readings)
    {
        const int width = 900;
        const int height = 340;

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(SKColors.White);

        using var font = new SKFont(SKTypeface.Default, 20f);
        using var boldFont = new SKFont(SKTypeface.FromFamilyName(null, SKFontStyle.Bold), 20f);

        if (readings.Count == 0)
        {
            using var emptyPaint = new SKPaint { Color = SKColors.Gray, IsAntialias = true };
            canvas.DrawText("Sem dados registrados neste período.", 20, height / 2f, SKTextAlign.Left, font, emptyPaint);
            return EncodeToPng(bitmap);
        }

        const float padding = 70f;
        var chartWidth = width - padding - 20f;
        var chartHeight = height - padding - 20f;

        var minValue = readings.Min(r => r.Value);
        var maxValue = readings.Max(r => r.Value);

        if (Math.Abs(maxValue - minValue) < 0.01f)
        {
            minValue -= 1f;
            maxValue += 1f;
        }

        var rangeValue = maxValue - minValue;

        var minDate = readings.Min(r => r.MeasuredAt);
        var maxDate = readings.Max(r => r.MeasuredAt);
        var rangeTime = (maxDate - minDate).TotalSeconds;
        if (rangeTime <= 0) rangeTime = 1;

        using var axisPaint = new SKPaint { Color = SKColors.LightGray, StrokeWidth = 2, IsAntialias = true };
        using var linePaint = new SKPaint { Color = SKColor.Parse("#5B4FE8"), StrokeWidth = 3, IsAntialias = true, Style = SKPaintStyle.Stroke };
        using var pointPaint = new SKPaint { Color = SKColor.Parse("#5B4FE8"), IsAntialias = true, Style = SKPaintStyle.Fill };
        using var textPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        using var highlightPaint = new SKPaint { Color = SKColors.DarkRed, IsAntialias = true };

        // Eixos
        canvas.DrawLine(padding, 10, padding, height - padding, axisPaint);
        canvas.DrawLine(padding, height - padding, width - 10, height - padding, axisPaint);

        // Labels do eixo Y
        canvas.DrawText(maxValue.ToString("F1"), 5, 28, SKTextAlign.Left, font, textPaint);
        canvas.DrawText(minValue.ToString("F1"), 5, height - padding, SKTextAlign.Left, font, textPaint);

        SKPoint MapPoint(SensorReading r)
        {
            var x = padding + (float)((r.MeasuredAt - minDate).TotalSeconds / rangeTime) * chartWidth;
            var y = 10 + chartHeight - (float)((r.Value - minValue) / rangeValue) * chartHeight;
            return new SKPoint(x, y);
        }

        var points = readings.Select(MapPoint).ToList();

        using var path = new SKPath();
        path.MoveTo(points[0]);
        for (var i = 1; i < points.Count; i++)
            path.LineTo(points[i]);

        canvas.DrawPath(path, linePaint);

        foreach (var p in points)
            canvas.DrawCircle(p, 4f, pointPaint);

        // Destaca ponto de valor máximo e mínimo
        var maxReading = readings.First(r => r.Value == maxValue);
        var minReading = readings.First(r => r.Value == minValue);
        var maxPoint = MapPoint(maxReading);
        var minPoint = MapPoint(minReading);

        canvas.DrawText($"Máx: {maxValue:F1} às {maxReading.MeasuredAt:HH:mm}", Math.Max(padding, maxPoint.X - 80), Math.Max(20, maxPoint.Y - 14), SKTextAlign.Left, boldFont, highlightPaint);
        canvas.DrawText($"Mín: {minValue:F1} às {minReading.MeasuredAt:HH:mm}", Math.Max(padding, minPoint.X - 80), Math.Min(height - padding - 10, minPoint.Y + 26), SKTextAlign.Left, font, textPaint);

        // Labels do eixo X (início e fim do período)
        canvas.DrawText(minDate.ToString("dd/MM HH:mm"), padding, height - 5, SKTextAlign.Left, font, textPaint);
        canvas.DrawText(maxDate.ToString("dd/MM HH:mm"), width - 160, height - 5, SKTextAlign.Left, font, textPaint);

        return EncodeToPng(bitmap);
    }

    private static byte[] EncodeToPng(SKBitmap bitmap)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private void BuildInfoCard(IContainer container, List<SensorReading> readings, SensorType type)
    {
        if (readings.Count == 0)
        {
            container.Background(Colors.Grey.Lighten3).Padding(8).Column(col =>
            {
                col.Item().Background(Colors.Grey.Medium).Padding(5).AlignCenter()
                    .Text("SEM DADOS").SemiBold().FontColor(Colors.White);

                col.Item().PaddingTop(6).Text("Nenhuma leitura registrada neste período.").FontSize(9);
            });

            return;
        }

        var average = readings.Average(r => r.Value);
        var min = readings.Min(r => r.Value);
        var max = readings.Max(r => r.Value);

        var (status, color, description) = EvaluateStatus(type, average, max);

        container.Background(Colors.Grey.Lighten4).Padding(8).Column(col =>
        {
            col.Item().Background(color).Padding(5).AlignCenter()
                .Text(status).SemiBold().FontColor(Colors.White);

            col.Item().PaddingTop(6).Text($"Média: {average:F1}{UnitSuffix(type)}").FontSize(10).SemiBold();
            col.Item().Text($"Mínima: {min:F1}{UnitSuffix(type)}").FontSize(10).SemiBold();
            col.Item().Text($"Máxima: {max:F1}{UnitSuffix(type)}").FontSize(10).SemiBold();

            col.Item().PaddingTop(6).Text(description).FontSize(9);
        });
    }

    private (string status, string color, string description) EvaluateStatus(SensorType type, double average, double max)
    {
        return type switch
        {
            SensorType.Temperature => max >= 32
                ? ("ALERTA CRÍTICO", Colors.Red.Darken2, "Risco: subida brusca de temperatura detectada, saindo da zona de estabilidade.")
                : average >= 29
                    ? ("ATENÇÃO", Colors.Orange.Medium, "Tendência de aquecimento detectada. Monitoramento intensificado recomendado.")
                    : ("IDEAL", Colors.Green.Medium, "Temperatura dentro da faixa considerada ideal."),

            SensorType.Humidity => average is < 40 or > 80
                ? ("ALERTA CRÍTICO", Colors.Red.Darken2, "Umidade fora da faixa segura para o lote.")
                : average is < 50 or > 70
                    ? ("ATENÇÃO", Colors.Orange.Medium, "Umidade se aproximando dos limites recomendados.")
                    : ("IDEAL", Colors.Green.Medium, "Umidade dentro da faixa considerada ideal."),

            SensorType.WaterLevel => average < 20
                ? ("ALERTA CRÍTICO", Colors.Red.Darken2, "Nível de água crítico. Verifique o abastecimento imediatamente.")
                : average < 40
                    ? ("ATENÇÃO", Colors.Orange.Medium, "Nível de água abaixo do recomendado. Monitoramento intensificado.")
                    : ("IDEAL", Colors.Green.Medium, "Nível de água dentro do esperado."),

            _ => ("INDEFINIDO", Colors.Grey.Medium, "Status não definido para este tipo de sensor.")
        };
    }

    private string UnitSuffix(SensorType type) => type switch
    {
        SensorType.Temperature => "°C",
        SensorType.Humidity => "%",
        SensorType.WaterLevel => "%",
        _ => string.Empty
    };

    private string TranslateType(SensorType type) => type switch
    {
        SensorType.Temperature => "Temperatura",
        SensorType.Humidity => "Umidade",
        SensorType.WaterLevel => "Nível de Água",
        _ => type.ToString()
    };
}
