using FarmSystemProject.Models.Farm;

namespace FarmSystemProject.DTOs.FarmDTO;

public class FarmResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int OwnerId { get; set; }
    public ICollection<LotSummaryResponse> Lots { get; set; } = [];
}