namespace FarmSystemProject.DTOs.Sales;

public class SaleRecordSummary
{
    public DateTime SaleDate { get; set; }
    public decimal UnitValue { get; set; }
    public int EggQuantity { get; set; }
    public decimal TotalCost { get; set; }
}
