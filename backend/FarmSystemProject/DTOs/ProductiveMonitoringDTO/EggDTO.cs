using FarmSystemProject.Models.Farm;

namespace FarmSystemProject.DTOs.ProductiveMonitoringDTO;
public class EggDTO
{
    public int Id { get; set; }
    public DateTime CollectDate { get; set; }
    public int CollectQuantity { get; set; }
    public int LotId { get; set; }
}
