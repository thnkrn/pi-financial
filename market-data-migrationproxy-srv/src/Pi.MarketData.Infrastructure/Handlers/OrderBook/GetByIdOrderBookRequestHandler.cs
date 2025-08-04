using Pi.MarketData.Application.Interfaces.OrderBook;
using Pi.MarketData.Application.Queries.OrderBook;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.OrderBook;

public class GetByIdOrderBookRequestHandler : GetByIdOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _orderBookService;

    public GetByIdOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> orderBookService)
    {
        _orderBookService = orderBookService;
    }

    protected override async Task<GetByIdOrderBookResponse> Handle(GetByIdOrderBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderBookService.GetByIdAsync(request.id);
            return new GetByIdOrderBookResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}