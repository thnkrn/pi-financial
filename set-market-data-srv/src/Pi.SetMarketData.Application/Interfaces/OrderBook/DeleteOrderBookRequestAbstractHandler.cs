using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.OrderBook;

namespace Pi.SetMarketData.Application.Interfaces.OrderBook;

public abstract class DeleteOrderBookRequestAbstractHandler: RequestHandler<DeleteOrderBookRequest, DeleteOrderBookResponse>;