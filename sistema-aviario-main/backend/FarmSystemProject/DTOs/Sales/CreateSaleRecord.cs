using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Sales;

public class CreateSaleRecord
{
    // Os limites do Range sao lidos com a cultura do servidor, e nao com a
    // do texto escrito aqui. Num container em pt-BR, "0.01" nao vira numero
    // e o cadastro de venda quebrava com erro 500 antes mesmo de validar.
    // ParseLimitsInInvariantCulture desliga essa dependencia do ambiente.
    [Required(ErrorMessage = "O valor unitário é obrigatório.")]
    [Range(typeof(decimal), "0.01", "2147483647", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true, ErrorMessage = "O valor unitário deve ser maior que zero.")]
    public decimal UnitValue { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int EggQuantity { get; set; }

    [Required(ErrorMessage = "A data de venda é obrigatória.")]
    public DateTime SaleDate { get; set; }

    [MaxLength(500, ErrorMessage = "A observação deve ter no máximo 500 caracteres.")]
    public string? Notes { get; set; }
}