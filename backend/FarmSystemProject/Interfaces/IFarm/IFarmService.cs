using FarmSystemProject.DTOs.FarmDTO;

namespace FarmSystemProject.Services.Interfaces.IFarm;

public interface IFarmService
{
    Task<FarmResponse> Create(int ownerId, CreateFarmRequest request);
    Task<FarmResponse> GetByOwnerId(int ownerId);
}