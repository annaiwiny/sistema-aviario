namespace FarmSystemProject.DTOs.ProductiveMonitoringDTO;

public class EggProductionResponse
{
    public int Id { get; set; }
    public DateTime ProductionDate { get; set; }
    public int Quantity { get; set; }
    public int LotId { get; set; }
    public decimal LayingRate { get; set; }
}
