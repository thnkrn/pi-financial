using Pi.MarketData.Application.Commands.OrderBook;
using Pi.MarketData.Application.Interfaces.OrderBook;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.OrderBook;

public class UpdateOrderBookRequestHandler : UpdateOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _OrderBookService;

    public UpdateOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> OrderBookService)
    {
        _OrderBookService = OrderBookService;
    }

    protected override async Task<UpdateOrderBookResponse> Handle(UpdateOrderBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _OrderBookService.UpdateAsync(request.id, request.OrderBook);
            return new UpdateOrderBookResponse(true, request.OrderBook);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}