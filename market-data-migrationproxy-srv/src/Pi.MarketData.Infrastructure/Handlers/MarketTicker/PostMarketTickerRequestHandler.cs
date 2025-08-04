using Pi.MarketData.Application.Interfaces.MarketTicker;
using Pi.MarketData.Application.Queries.MarketTicker;
using Pi.MarketData.Application.Services.MarketData.MarketTicker;
using Pi.MarketData.Domain.Models.Response.MorningStar;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketTicker;

public class PostMarketTickerRequestHandler : PostMarketTickerRequestAbstractHandler
{
    private readonly IMongoService<IncomeStatement> _incomeStatementService;
    private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDetailService;
    private readonly IMongoService<Domain.Entities.OrderBook> _orderBookService;
    private readonly IMongoService<Domain.Entities.PriceInfo> _priceInfoService;
    private readonly IMongoService<ValuationRations> _valuationRationService;


    public PostMarketTickerRequestHandler(
        IMongoService<Domain.Entities.PriceInfo> priceInfoService,
        IMongoService<ValuationRations> valuationRationService,
        IMongoService<IncomeStatement> incomeStatementService,
        IMongoService<Domain.Entities.OrderBook> orderBookService,
        IMongoService<Domain.Entities.InstrumentDetail> instrumentDetailService
    )
    {
        _priceInfoService = priceInfoService;
        _valuationRationService = valuationRationService;
        _incomeStatementService = incomeStatementService;
        _orderBookService = orderBookService;
        _instrumentDetailService = instrumentDetailService;
    }

    protected override async Task<PostMarketTickerResponse> Handle(
        PostMarketTickerRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            List<Domain.Entities.PriceInfo> _priceInfos = [];
            List<ValuationRations> _valuationRations = [];
            List<IncomeStatement> _incomeStatements = [];
            List<Domain.Entities.OrderBook> _orderBooks = [];
            List<string> venue = [];
            List<Domain.Entities.InstrumentDetail> _instrumentDetails = [];


            for (var i = 0; i < request.data.Param?.Count; i++)
            {
                var _priceInfo =
                    await _priceInfoService.GetBySymbolAsync(request.data.Param[i].Symbol ?? "")
                    ?? new Domain.Entities.PriceInfo();

                var _valuationRation =
                    await _valuationRationService.GetMongoBySymbolAsync(
                        request.data.Param[i].Symbol ?? ""
                    ) ?? new ValuationRations();

                var incomeStatement =
                    await _incomeStatementService.GetMongoBySymbolAsync(
                        request.data.Param[i].Symbol ?? ""
                    ) ?? new IncomeStatement();

                var orderBook =
                    await _orderBookService.GetBySymbolAsync(request.data.Param[i].Symbol ?? "")
                    ?? new Domain.Entities.OrderBook();

                var instrumentDetail =
                    await _instrumentDetailService.GetMongoBySymbolAsync(request.data.Param[i].Symbol ?? "");

                _priceInfos.Add(_priceInfo);
                _valuationRations.Add(_valuationRation);
                _incomeStatements.Add(incomeStatement);
                venue.Add(request.data.Param[i].Venue ?? "");
                _orderBooks.Add(orderBook);
                _instrumentDetails.Add(instrumentDetail);
            }

            var result = MarketTickerService.GetResult(
                _priceInfos,
                _valuationRations,
                _incomeStatements,
                venue,
                _orderBooks.First(),
                _instrumentDetails
            );

            return new PostMarketTickerResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}