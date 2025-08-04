using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Queries.OrderBook;
public record PostOrderBookResponse(MarketOrderBookResponse Data);
