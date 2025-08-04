using Pi.Client.ItBackOffice.Model;

namespace Pi.TfexService.Application.Services.It;

public sealed record PaginatedOrderTrade(
    List<TradeDetail> TradeDetails,
    bool HasNextPage
);