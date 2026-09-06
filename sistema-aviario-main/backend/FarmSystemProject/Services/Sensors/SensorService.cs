using FarmSystemProject.Data;
using FarmSystemProject.DTOs.Sensors;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.ISensors;
using FarmSystemProject.Models.Sensors;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.Sensors;

public class SensorService : ISensorService
{
    private readonly AppDbContext _context;

    public SensorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SensorSummary>> GetSensorsSummary(int lotId, int ownerId)
    {
        var farmId = await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => f.Id)
            .FirstOrDefaultAsync();

        if (farmId == 0)
            throw new NotFoundException("Você não possui nenhum aviário cadastrado");

        var lot = await _context.Lots
            .FirstOrDefaultAsync(l => l.Id == lotId && l.FarmId == farmId);

        if (lot == null)
            throw new NotFoundException("Lote não encontrado");

        var activeSensors = await _context.Sensors
            .AsNoTracking()
            .Where(s => s.LotId == lotId && s.IsActive)
            .Select(s => new
            {
                s.Type,

                LatestReading = _context.SensorReadings
                    .Where(sr => sr.SensorId == s.Id)
                    .OrderByDescending(sr => sr.MeasuredAt)
                    .FirstOrDefault()
            })
            .ToListAsync();

        var sensorTypes = Enum.GetValues<SensorType>();
        var sensorsSummary = new List<SensorSummary>();

        foreach (var type in sensorTypes)
        {
            var dataFromType = activeSensors
                .Where(s => s.Type == type && s.LatestReading != null)
                .ToList();

            if (dataFromType.Count > 0)
            {
                var averageValue = dataFromType.Average(s => s.LatestReading!.Value);
                var latestData = dataFromType.Max(s => s.LatestReading!.MeasuredAt);
                var status = CalculateSensorStatus(type, averageValue);
                var unit = GetUnitSuffix(type);

                sensorsSummary.Add(new SensorSummary
                {
                    Type = TranslateSensorType(type),
                    Value = $"{averageValue:F1} {unit}",
                    MeasuredAt = latestData,
                    Status = status
                });
            }

            // Se a lista está vazia, então não há dados para este tipo de sensor.
            else
            {
                sensorsSummary.Add(new SensorSummary
                {
                    Type = TranslateSensorType(type),
                    Value = null,
                    MeasuredAt = null,
                    Status = "Sem Dados"
                });
            }
        }

        return sensorsSummary;
    }

    public async Task RegisterEsp32Readings(Esp32Payload payload)
    {
        var Esp32Sensors = await _context.Sensors
        .Where(s => s.MacAddress == payload.MacAddress)
        .ToListAsync();

        if (!Esp32Sensors.Any())
            throw new NotFoundException($"Nenhum sensor encontrado para o MAC Address '{payload.MacAddress}'.");

        var now = DateTime.Now;

        foreach (var reading in payload.Readings)
        {
            var sensor = Esp32Sensors.FirstOrDefault(s => s.Type == reading.Type);

            if (sensor == null)
                continue;

            _context.SensorReadings.Add(
                new SensorReading
                {
                    SensorId = sensor.Id,
                    Value = reading.Value,
                    MeasuredAt = now
                }
            );
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Sensor> Create(CreateSensor request)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == request.LotId))
            throw new NotFoundException("Lote não encontrado.");

        if (!Enum.IsDefined(typeof(SensorType), request.Type))
            throw new BusinessException("Tipo de sensor inválido. Tipos permitidos: 1 (Temperatura), 2 (Umidade), 3 (Nível de Água).");

        // Evita duplicidade
            var existingSensor = await _context.Sensors
            .FirstOrDefaultAsync(s => s.MacAddress == request.MacAddress && s.Type == request.Type && s.LotId == request.LotId);

        if (existingSensor != null)
            throw new BusinessException("Já existe um sensor deste tipo e endereço MAC cadastrado para este Lote.");

        var sensor = new Sensor
        {
            MacAddress = request.MacAddress,
            Type = request.Type,
            LotId = request.LotId,
            IsActive = true
        };

        _context.Sensors.Add(sensor);
        await _context.SaveChangesAsync();

        return sensor;
    }
    public async Task<List<SensorReading>> GetReadingsHistory(int lotId, int ownerId, SensorType type, DateTime from)
    {
        var farmId = await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => f.Id)
            .FirstOrDefaultAsync();

        if (farmId == 0)
            throw new NotFoundException("Você não possui nenhum aviário cadastrado");

        var lot = await _context.Lots
            .FirstOrDefaultAsync(l => l.Id == lotId && l.FarmId == farmId);

        if (lot == null)
            throw new NotFoundException("Lote não encontrado");

        var sensorIds = await _context.Sensors
            .Where(s => s.LotId == lotId && s.Type == type && s.IsActive)
            .Select(s => s.Id)
            .ToListAsync();

        if (!sensorIds.Any())
            throw new NotFoundException("Nenhum sensor ativo encontrado para este tipo de sensor no lote informado.");

        var readings = await _context.SensorReadings
            .AsNoTracking()
            .Where(r => sensorIds.Contains(r.SensorId) && r.MeasuredAt >= from)
            .OrderBy(r => r.MeasuredAt)
            .ToListAsync();

        return readings;
    }

    public string CalculateSensorStatus(SensorType type, double value)
    {
        return type switch
        {
            SensorType.Temperature => value switch
            {
                >= 18 and <= 24 => "Ideal",
                > 24 and <= 28 => "Atenção",
                _ => "Crítico" // < 18 ou > 28
            },

            SensorType.Humidity => value switch
            {
                >= 50 and <= 70 => "Ideal",
                > 70 and <= 80 => "Atenção",
                _ => "Crítico" // < 50 ou > 80
            },

            SensorType.WaterLevel => value switch
            {
                >= 41 => "Ideal",
                > 20 and < 41 => "Atenção",
                _ => "Crítico" // <= 20
            },

            _ => "Indefinido"
        };
    }

    public string TranslateSensorType(SensorType type) => type switch
    {
        SensorType.Temperature => "Temperatura",
        SensorType.Humidity => "Umidade",
        SensorType.WaterLevel => "Nível de Água",
        _ => type.ToString()
    };

    public string GetUnitSuffix(SensorType type) => type switch
    {
        SensorType.Temperature => "°C",
        SensorType.Humidity => "%",
        SensorType.WaterLevel => "%",
        _ => string.Empty
    };
}