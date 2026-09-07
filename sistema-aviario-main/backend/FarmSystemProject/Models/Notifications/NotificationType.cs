namespace FarmSystemProject.Models.Notifications;

public enum NotificationType
{
    // Mortalidade diária acima do limite aceitável para o plantel vivo
    Mortality = 1,

    // Queda brusca na postura em relação à média dos últimos dias
    LayingDrop = 2,

    // Leitura de sensor em faixa crítica (temperatura, umidade ou nível de água)
    Sensor = 3
}
