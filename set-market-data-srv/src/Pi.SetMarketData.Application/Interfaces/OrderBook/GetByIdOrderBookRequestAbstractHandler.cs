using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.OrderBook;

namespace Pi.SetMarketData.Application.Interfaces.OrderBook;

public abstract class GetByIdOrderBookRequestAbstractHandler: RequestHandler<GetByIdOrderBookRequest, GetByIdOrderBookResponse>;