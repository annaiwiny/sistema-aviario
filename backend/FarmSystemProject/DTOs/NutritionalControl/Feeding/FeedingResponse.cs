namespace FarmSystemProject.DTOs.NutritionalControl.Feeding;

public class FeedingResponse
{
    public int Id { get; set; }
    public decimal ConsumptionQuantity { get; set; }
    public DateTime ConsumptionDate { get; set; }
    public int LotId { get; set; }
}