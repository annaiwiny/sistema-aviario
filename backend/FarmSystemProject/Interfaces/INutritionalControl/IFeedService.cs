using FarmSystemProject.DTOs.NutritionalControl.Feed;

namespace FarmSystemProject.Interfaces.INutritionalControl;

public interface IFeedService
{
    Task<FeedResponse> Create(int lotId, int ownerId, CreateFeed request);
    Task<FeedSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date);
    Task<IEnumerable<FeedResponse>> GetAllByLotId(int lotId, int ownerId);
}
