using FarmSystemProject.DTOs.Lots;

namespace FarmSystemProject.Interfaces.ILots;
public interface ILotService
{
    Task<LotResponse> Create(int ownerId, CreateLotRequest request);
    Task<LotResponse> GetById(int id, int ownerId);
    Task<LotResponse> UpdateAsync(int id, int ownerId, UpdateLotRequest request);
}