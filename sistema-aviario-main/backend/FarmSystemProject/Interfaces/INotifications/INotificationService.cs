using FarmSystemProject.DTOs.Notifications;
using FarmSystemProject.Models.Sensors;

namespace FarmSystemProject.Interfaces.INotifications;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetAll(int ownerId);
    Task<int> GetUnreadCount(int ownerId);
    Task MarkAsRead(int notificationId, int ownerId);
    Task MarkAllAsRead(int ownerId);
    Task Delete(int notificationId, int ownerId);

    // Verificações de anormalidade. São chamadas pelos serviços de registro e só
    // geram notificação quando os dados fogem do esperado.
    Task CheckMortality(int lotId, DateTime referenceDate);
    Task CheckEggProduction(int lotId, DateTime referenceDate);
    Task CheckSensorReading(int lotId, SensorType type, string status, string formattedValue, DateTime measuredAt);
}
