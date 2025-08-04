using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketOrderBook;

public abstract class PostOrderBookAbstractHandler
    : RequestHandler<PostOrderBookRequest, PostOrderBookResponse>;
