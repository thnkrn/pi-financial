using Pi.MarketData.Application.Commands.OrderBook;
using Pi.MarketData.Application.Interfaces.OrderBook;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.OrderBook;

public class DeleteOrderBookRequestHandler : DeleteOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _orderBookService;

    public DeleteOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> orderBookService)
    {
        _orderBookService = orderBookService;
    }

    protected override async Task<DeleteOrderBookResponse> Handle(DeleteOrderBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _orderBookService.DeleteAsync(request.id);
            return new DeleteOrderBookResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}