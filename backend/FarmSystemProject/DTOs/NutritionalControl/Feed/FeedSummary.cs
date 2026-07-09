namespace FarmSystemProject.DTOs.NutritionalControl.Feed;

public class FeedSummary
{
    public DateTime PurchaseDate { get; set; }
    public decimal BagWeight { get; set; }
    public decimal BagValue { get; set; }
    public int BagQuantity { get; set; }
    public decimal TotalCost { get; set; }
}
