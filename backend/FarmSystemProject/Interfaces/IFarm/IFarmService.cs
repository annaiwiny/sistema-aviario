using FarmSystemProject.DTOs.FarmDTO;

namespace FarmSystemProject.Services.Interfaces.IFarm;

public interface IFarmService
{
    Task<FarmResponse> CreateAsync(int ownerId, CreateFarmRequest request);
    Task<FarmResponse?> GetByOwnerIdAsync(int ownerId);
}