using FarmSystemProject.DTOs.NutritionalControl.Feeding;

namespace FarmSystemProject.Interfaces.INutritionalControl;

public interface IFeedingService
{
    Task<FeedingResponse> Create(int lotId, int ownerId, CreateFeeding request);
    Task<FeedingSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date);
    Task<IEnumerable<FeedingResponse>> GetAllByLotId(int lotId, int ownerId);
}