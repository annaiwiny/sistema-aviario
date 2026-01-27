using FarmSystemProject.DTOs;

namespace FarmSystemProject.Interfaces;
public interface IRaceService
{
    Task<IEnumerable<RaceDTO>> GetAll();
    Task<RaceDTO?> GetById(int id);
    Task<RaceDTO> Create(RaceDTO raceDto);
    Task Update(int id, RaceDTO raceDto);
    Task Delete(int id);
}
