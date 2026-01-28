using FarmSystemProject.DTOs.FarmDTO;

namespace FarmSystemProject.Interfaces.IFarm;
public interface ILotService
{
    Task<IEnumerable<LotDTO>> GetAll();
    Task<LotDTO?> GetById(int id);
    Task<LotDTO> Create(LotDTO lotDto);
}
