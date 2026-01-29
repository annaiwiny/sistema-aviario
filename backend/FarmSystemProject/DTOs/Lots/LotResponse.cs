namespace FarmSystemProject.DTOs.Lots;

public class LotResponse
{
    public int Id { get; set; }
    public DateTime AccommodationDate { get; set; }
    public int FarmId { get; set; }
    public int TotalQuantity => Lineages.Sum(x => x.Quantity);
    public List<LineageResponse> Lineages { get; set; } = [];
}