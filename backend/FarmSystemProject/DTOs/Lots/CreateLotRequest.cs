using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Lots;

public class CreateLotRequest
{
    [Required(ErrorMessage = "A data de alojamento é obrigatória.")]
    public DateTime AccommodationDate { get; set; }

    [Required(ErrorMessage = "É necessário informar pelo menos uma linhagem.")]
    public List<CreateLineageRequest> Lineages { get; set; } = [];
}