using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.It;

namespace Pi.TfexService.Application.Queries.Order;

public interface IItOrderTradeQueries
{
    Task<PaginatedOrderTrade?> GetOrderTrade(
        GetTradeDetailRequestModel requestModel,
        CancellationToken cancellationToken = default
    );
}