using Pi.SetMarketData.Application.Interfaces.OrderBook;
using Pi.SetMarketData.Application.Queries.OrderBook;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.OrderBook;

public class GetOrderBookRequestHandler: GetOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _OrderBookService;
    
    public GetOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> OrderBookService)
    {
        _OrderBookService = OrderBookService;
    }
    
    protected override async Task<GetOrderBookResponse> Handle(GetOrderBookRequest request, CancellationToken cancellationToken)
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