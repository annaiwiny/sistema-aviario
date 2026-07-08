namespace FarmSystemProject.DTOs.Sales;

public class SaleRecordResponse
{
    public int Id { get; set; }
    public decimal UnitValue { get; set; }
    public int EggQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime SaleDate { get; set; }
    public int LotId { get; set; }
}