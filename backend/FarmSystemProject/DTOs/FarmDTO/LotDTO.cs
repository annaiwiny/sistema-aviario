namespace FarmSystemProject.DTOs.FarmDTO;
public class LotDTO
{
    public int Id { get; set; }
    public DateTime AccommodationDate { get; set; }
    public int FarmId { get; set; }
    public List<LotItemDTO> Items { get; set; } = new();
}
public class LotItemDTO
{
    public string Race { get; set; } = null!;
    public int Quantity { get; set; }
}
