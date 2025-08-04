using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.OrderBook;

namespace Pi.SetMarketData.Application.Interfaces.OrderBook;

public abstract class CreateOrderBookRequestAbstractHandler: RequestHandler<CreateOrderBookRequest, CreateOrderBookResponse>;