using Pi.MarketData.Application.Commands.OrderBook;
using Pi.MarketData.Application.Interfaces.OrderBook;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.OrderBook;

public class CreateOrderBookRequestHandler : CreateOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _orderBookService;

    public CreateOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> orderBookService)
    {
        _orderBookService = orderBookService;
    }

    protected override async Task<CreateOrderBookResponse> Handle(CreateOrderBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _orderBookService.CreateAsync(request.OrderBook);
            return new CreateOrderBookResponse(true, request.OrderBook);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}