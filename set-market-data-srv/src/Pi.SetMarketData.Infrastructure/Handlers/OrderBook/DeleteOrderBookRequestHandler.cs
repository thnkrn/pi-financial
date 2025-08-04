using Pi.SetMarketData.Application.Commands.OrderBook;
using Pi.SetMarketData.Application.Interfaces.OrderBook;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.OrderBook;

public class DeleteOrderBookRequestHandler : DeleteOrderBookRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.OrderBook> _orderBookService;

    public DeleteOrderBookRequestHandler(IMongoService<Domain.Entities.OrderBook> orderBookService)
    {
        _orderBookService = orderBookService;
    }

    protected override async Task<DeleteOrderBookResponse> Handle(DeleteOrderBookRequest request, CancellationToken cancellationToken)
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