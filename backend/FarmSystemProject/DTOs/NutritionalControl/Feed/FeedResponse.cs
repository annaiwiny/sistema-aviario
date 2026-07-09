namespace FarmSystemProject.DTOs.NutritionalControl.Feed;

public class FeedResponse
{
    public int Id { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal BagWeight { get; set; }
    public int BagQuantity { get; set; }
    public decimal BagValue { get; set; }
    public decimal TotalCost { get; set; }
    public int LotId { get; set; }
}
