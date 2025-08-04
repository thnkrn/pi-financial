using Pi.SetMarketData.Application.Queries.OrderBook;
using Pi.SetMarketData.Application.Interfaces.OrderBook;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.OrderBook;

public class GetByIdOrderBookRequestHandler : GetByIdOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _orderBookService;

    public GetByIdOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> orderBookService)
    {
        _orderBookService = orderBookService;
    }

    protected override async Task<GetByIdOrderBookResponse> Handle(GetByIdOrderBookRequest request, CancellationToken cancellationToken)
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