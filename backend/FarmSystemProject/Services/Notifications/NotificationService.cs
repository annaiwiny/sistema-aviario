using FarmSystemProject.Data;
using FarmSystemProject.DTOs.Notifications;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.INotifications;
using FarmSystemProject.Models.Notifications;
using FarmSystemProject.Models.Sensors;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    // Limite de mortalidade diária (% do plantel vivo no dia) considerado anormal.
    private const decimal DailyMortalityRateThreshold = 1.0m;

    // Queda mínima na postura (em relação à média recente) para gerar alerta.
    private const decimal LayingDropThreshold = 0.20m;

    // Janela e mínimo de dias com registro necessários para calcular a média de postura.
    private const int LayingHistoryDays = 7;
    private const int LayingMinHistoryDays = 3;

    // Intervalo mínimo entre dois alertas do mesmo sensor, para não inundar o app.
    private static readonly TimeSpan SensorAlertCooldown = TimeSpan.FromHours(6);

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationResponse>> GetAll(int ownerId)
    {
        var farmId = await GetFarmId(ownerId);

        if (farmId == 0)
            return [];

        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.FarmId == farmId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(n => new NotificationResponse
        {
            Id = n.Id,
            Type = TranslateType(n.Type),
            Title = n.Title,
            Description = n.Description,
            CreatedAt = n.CreatedAt,
            IsRead = n.IsRead,
            LotId = n.LotId
        });
    }

    public async Task<int> GetUnreadCount(int ownerId)
    {
        var farmId = await GetFarmId(ownerId);

        if (farmId == 0)
            return 0;

        return await _context.Notifications
            .CountAsync(n => n.FarmId == farmId && !n.IsRead);
    }

    public async Task MarkAsRead(int notificationId, int ownerId)
    {
        var farmId = await GetFarmId(ownerId);

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.FarmId == farmId);

        if (notification == null)
            throw new NotFoundException("Notificação não encontrada.");

        if (notification.IsRead)
            return;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsRead(int ownerId)
    {
        var farmId = await GetFarmId(ownerId);

        if (farmId == 0)
            return;

        var unread = await _context.Notifications
            .Where(n => n.FarmId == farmId && !n.IsRead)
            .ToListAsync();

        if (unread.Count == 0)
            return;

        foreach (var notification in unread)
            notification.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int notificationId, int ownerId)
    {
        var farmId = await GetFarmId(ownerId);

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.FarmId == farmId);

        if (notification == null)
            throw new NotFoundException("Notificação não encontrada.");

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
    }

    public async Task CheckMortality(int lotId, DateTime referenceDate)
    {
        var lot = await _context.Lots
            .AsNoTracking()
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == lotId);

        if (lot == null)
            return;

        var initialStock = lot.Lineages.Sum(x => x.Quantity);

        // Aves perdidas antes do dia analisado
        var previousLosses = await _context.Mortalities
            .Where(m => m.LotId == lotId && m.DateDeath.Date < referenceDate.Date)
            .SumAsync(m => m.DeathQuantity + m.CutQuantity);

        var birdsAlive = initialStock - previousLosses;

        if (birdsAlive <= 0)
            return;

        var deathsOnDate = await _context.Mortalities
            .Where(m => m.LotId == lotId && m.DateDeath.Date == referenceDate.Date)
            .SumAsync(m => m.DeathQuantity);

        if (deathsOnDate <= 0)
            return;

        var rate = Math.Round(((decimal)deathsOnDate / birdsAlive) * 100, 2);

        if (rate < DailyMortalityRateThreshold)
            return;

        await Register(
            lot.FarmId,
            lotId,
            NotificationType.Mortality,
            "Alta Taxa de Mortalidade",
            $"Lote {lotId:00} registrou {rate}% de mortalidade em {referenceDate:dd/MM} ({deathsOnDate} de {birdsAlive} aves).",
            referenceDate);
    }

    public async Task CheckEggProduction(int lotId, DateTime referenceDate)
    {
        var lot = await _context.Lots
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lotId);

        if (lot == null)
            return;

        var totalOnDate = await _context.EggProductions
            .Where(e => e.LotId == lotId && e.ProductionDate.Date == referenceDate.Date)
            .SumAsync(e => e.Quantity);

        // Média diária dos dias anteriores que possuem registro
        var history = await _context.EggProductions
            .Where(e => e.LotId == lotId
                     && e.ProductionDate.Date < referenceDate.Date
                     && e.ProductionDate.Date >= referenceDate.Date.AddDays(-LayingHistoryDays))
            .GroupBy(e => e.ProductionDate.Date)
            .Select(g => g.Sum(e => e.Quantity))
            .ToListAsync();

        if (history.Count < LayingMinHistoryDays)
            return;

        var average = (decimal)history.Average();

        if (average <= 0)
            return;

        var drop = (average - totalOnDate) / average;

        if (drop < LayingDropThreshold)
            return;

        var dropPercentage = Math.Round(drop * 100, 1);

        await Register(
            lot.FarmId,
            lotId,
            NotificationType.LayingDrop,
            "Queda Brusca na Postura",
            $"Lote {lotId:00} produziu {totalOnDate} ovos em {referenceDate:dd/MM}, {dropPercentage}% abaixo da média recente ({Math.Round(average)} ovos/dia).",
            referenceDate);
    }

    public async Task CheckSensorReading(int lotId, SensorType type, string status, string formattedValue, DateTime measuredAt)
    {
        if (status != "Crítico")
            return;

        var farmId = await _context.Lots
            .Where(l => l.Id == lotId)
            .Select(l => l.FarmId)
            .FirstOrDefaultAsync();

        if (farmId == 0)
            return;

        var typeName = TranslateSensorType(type);

        await Register(
            farmId,
            lotId,
            NotificationType.Sensor,
            $"{typeName} em Nível Crítico",
            $"Lote {lotId:00} está com {typeName.ToLower()} em {formattedValue}, fora da faixa segura.",
            measuredAt,
            SensorAlertCooldown);
    }

    // Cria a notificação evitando repetir o mesmo alerta. Sem cooldown, vale um alerta
    // por dia de referência; com cooldown, um alerta por janela de tempo.
    private async Task Register(
        int farmId,
        int? lotId,
        NotificationType type,
        string title,
        string description,
        DateTime referenceDate,
        TimeSpan? cooldown = null)
    {
        var existing = _context.Notifications
            .Where(n => n.FarmId == farmId && n.LotId == lotId && n.Type == type && n.Title == title);

        bool alreadyNotified;

        if (cooldown.HasValue)
        {
            var limit = DateTime.Now - cooldown.Value;
            alreadyNotified = await existing.AnyAsync(n => n.CreatedAt >= limit);
        }
        else
        {
            alreadyNotified = await existing.AnyAsync(n => n.ReferenceDate.Date == referenceDate.Date);
        }

        if (alreadyNotified)
            return;

        _context.Notifications.Add(new Notification
        {
            FarmId = farmId,
            LotId = lotId,
            Type = type,
            Title = title,
            Description = description,
            ReferenceDate = referenceDate,
            CreatedAt = DateTime.Now,
            IsRead = false
        });

        await _context.SaveChangesAsync();
    }

    private async Task<int> GetFarmId(int ownerId)
    {
        return await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => f.Id)
            .FirstOrDefaultAsync();
    }

    private static string TranslateType(NotificationType type) => type switch
    {
        NotificationType.Mortality => "alert",
        NotificationType.LayingDrop => "production",
        NotificationType.Sensor => "sensor",
        _ => "info"
    };

    private static string TranslateSensorType(SensorType type) => type switch
    {
        SensorType.Temperature => "Temperatura",
        SensorType.Humidity => "Umidade",
        SensorType.WaterLevel => "Nível de Água",
        _ => type.ToString()
    };
}
