using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Lots;

public class UpdateLotRequest
{
    public DateTime? AccommodationDate { get; set; }

    public List<CreateLineageRequest>? Lineages { get; set; }
}