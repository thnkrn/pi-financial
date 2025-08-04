using Pi.SetMarketData.Application.Commands.OrderBook;
using Pi.SetMarketData.Application.Interfaces.OrderBook;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.OrderBook;

public class CreateOrderBookRequestHandler : CreateOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _orderBookService;

    public CreateOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> orderBookService)
    {
        _orderBookService = orderBookService;
    }

    protected override async Task<CreateOrderBookResponse> Handle(CreateOrderBookRequest request, CancellationToken cancellationToken)
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