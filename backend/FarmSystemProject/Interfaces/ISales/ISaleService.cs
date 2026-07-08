using FarmSystemProject.DTOs.Sales;

namespace FarmSystemProject.Interfaces.ISales;

public interface ISaleService
{
    Task<SaleRecordResponse> Create(int lotId, int ownerId, CreateSaleRecord request);
    Task<SaleRecordSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date);
    Task<IEnumerable<SaleRecordResponse>> GetAllByLotId(int lotId, int ownerId);
}