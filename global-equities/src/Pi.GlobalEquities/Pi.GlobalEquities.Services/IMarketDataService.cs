using Pi.GlobalEquities.DomainModels.MarketData;

namespace Pi.GlobalEquities.Services;

public interface IMarketDataService
{
    Task<Dictionary<OrderType, IEnumerable<OrderDuration>>> GetSupportedOrderDetails(string symbolId, CancellationToken ct);
}
