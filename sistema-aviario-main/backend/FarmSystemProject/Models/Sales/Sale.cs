using FarmSystemProject.Models.Farms;
using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Sales;
public class Sale
{
    [Key]
    public int Id { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal UnitValue { get; set; }

    [Required]
    public int EggQuantity { get; set; }

    // Atributo auxiliar (Não cria coluna no banco)
    [NotMapped]
    public decimal TotalValue => UnitValue * EggQuantity;

    [Required]
    public DateTime SaleDate { get; set; }

    // Campo livre e opcional: justifica ovos colhidos que não foram vendidos
    [MaxLength(500)]
    public string? Notes { get; set; }

    // A venda pertence à GRANJA, não ao lote: os ovos de todos os lotes são
    // juntados antes de vender, então não há como dizer de qual lote saiu cada
    // dúzia. Este é o vínculo obrigatório.
    [Required]
    public int FarmId { get; set; }

    [ForeignKey("FarmId")]
    public Farm Farm { get; set; } = null!;

    // Opcional e mantido apenas por causa das vendas antigas, lançadas quando a
    // tela ficava dentro do lote. Vendas novas entram sem lote nenhum.
    public int? LotId { get; set; }

    [ForeignKey("LotId")]
    public Lot? Lot { get; set; }
}
