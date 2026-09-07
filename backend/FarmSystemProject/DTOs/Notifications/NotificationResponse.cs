namespace FarmSystemProject.DTOs.Notifications;

public class NotificationResponse
{
    public int Id { get; set; }

    // Chave usada pelo app para escolher o ícone: "alert", "production" ou "sensor"
    public string Type { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public int? LotId { get; set; }
}
