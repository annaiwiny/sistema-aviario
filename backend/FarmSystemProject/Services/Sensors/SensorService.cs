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

                sensorsSummary.Add(new SensorSummary
                {
                    Type = type.ToString(),
                    Value = averageValue,
                    MeasuredAt = latestData,
                    Status = "Indefinido"
                });
            }

            // Se a lista está vazia, então não há dados para este tipo de sensor.
            else
            {
                sensorsSummary.Add(new SensorSummary
                {
                    Type = type.ToString(),
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
            return;

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
}