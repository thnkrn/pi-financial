using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.OrderBook;


namespace Pi.SetMarketData.Application.Interfaces.MarketOrderBook;

public abstract class PostOrderBookAbstractHandler
    : RequestHandler<PostOrderBookRequest, PostOrderBookResponse>;
