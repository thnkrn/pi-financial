using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;

namespace Pi.TfexService.Application.Queries.Order;

public interface ISetTradeOrderQueries
{
    Task<PaginatedSetTradeOrder> GetOrders(string accountCode, int page, int pageSize,
        string? sort = null,
        Side? side = null,
        DateOnly? dateFrom = null,
        DateOnly? dateTo = null,
        CancellationToken cancellationToken = default);
    Task<List<SetTradeOrder>> GetActiveOrders(string accountCode, string? sid, string? sort = null, CancellationToken cancellationToken = default);
    Task<SetTradeOrder> GetOrder(string accountCode, string orderNo, CancellationToken cancellationToken);
}