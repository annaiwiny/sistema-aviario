using FarmSystemProject.DTOs.Sales;

namespace FarmSystemProject.Interfaces.ISales;

// As vendas são da GRANJA: os ovos de todos os lotes são juntados antes de
// vender, então não existe venda "de um lote". Por isso todo método aqui é
// resolvido pelo dono (ownerId) e não por lote.
public interface ISaleService
{
    Task<SaleRecordResponse> Create(int ownerId, CreateSaleRecord request);
    Task<SaleRecordSummary> GetSummaryByDate(int ownerId, DateTime date);
    Task<IEnumerable<SaleRecordResponse>> GetAllByFarm(int ownerId);
}
