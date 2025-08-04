using Pi.MarketData.Application.Interfaces.OrderBook;
using Pi.MarketData.Application.Queries.OrderBook;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.OrderBook;

public class GetOrderBookRequestHandler : GetOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _OrderBookService;

    public GetOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> OrderBookService)
    {
        _OrderBookService = OrderBookService;
    }

    protected override async Task<GetOrderBookResponse> Handle(GetOrderBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _OrderBookService.GetAllAsync();
            return new GetOrderBookResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}