namespace FarmSystemProject.DTOs.Sales;

public class SaleRecordSummary
{
    public DateTime SaleDate { get; set; }
    public decimal UnitValue { get; set; }
    public int EggQuantity { get; set; }
    public decimal TotalValue { get; set; }

    // Observações do dia, unidas quando há mais de uma venda na mesma data
    public string? Notes { get; set; }
}
