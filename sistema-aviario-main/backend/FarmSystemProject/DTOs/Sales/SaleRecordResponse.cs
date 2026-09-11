namespace FarmSystemProject.DTOs.Sales;

public class SaleRecordResponse
{
    public int Id { get; set; }
    public decimal UnitValue { get; set; }
    public int EggQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime SaleDate { get; set; }
    public string? Notes { get; set; }
    public int FarmId { get; set; }

    // Preenchido só nas vendas antigas, lançadas quando a tela ficava dentro
    // do lote. Nas novas vem nulo: a venda é da granja inteira.
    public int? LotId { get; set; }
}
