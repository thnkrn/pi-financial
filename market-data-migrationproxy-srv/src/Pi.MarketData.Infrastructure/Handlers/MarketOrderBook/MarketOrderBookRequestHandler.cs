using Pi.MarketData.Application.Interfaces.MarketOrderBook;
using Pi.MarketData.Application.Queries.OrderBook;
using Pi.MarketData.Application.Services.MarketData.MarketOrderBook;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers;

public class MarketOrderBookRequestHandler : PostOrderBookAbstractHandler
{
    private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDetailService;
    private readonly IMongoService<Domain.Entities.OrderBook> _orderBookService;

    public MarketOrderBookRequestHandler(
        IMongoService<Domain.Entities.OrderBook> ordeerBookService,
        IMongoService<Domain.Entities.InstrumentDetail> instrumentDetailService
    )
    {
        _orderBookService = ordeerBookService;
        _instrumentDetailService = instrumentDetailService;
    }

    protected override async Task<PostOrderBookResponse> Handle(PostOrderBookRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            List<Domain.Entities.OrderBook> _orderBooks = [];
            List<Domain.Entities.InstrumentDetail> _instrumentDetails = [];

            for (var i = 0; i < request.data.SymbolVenueList?.Count; i++)
            {
                var orderBookResponse =
                    await _orderBookService.GetMongoBySymbolAsync(request.data.SymbolVenueList[i].Symbol ?? "")
                    ?? new Domain.Entities.OrderBook();
                var instrumentDetail =
                    await _instrumentDetailService.GetMongoBySymbolAsync(request.data.SymbolVenueList[i].Symbol ?? "")
                    ?? new Domain.Entities.InstrumentDetail();

                _orderBooks.Add(orderBookResponse);
                _instrumentDetails.Add(instrumentDetail);
            }

            var result = MarketOrderBookService.GetResult(_orderBooks, _instrumentDetails);

            return new PostOrderBookResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}