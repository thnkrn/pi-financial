using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.DataProcessingService.Handlers;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
public class KafkaMessageHandlerDependencies
{
    /// <summary>
    /// </summary>
    /// <param name="instrumentDetailService"></param>
    /// <param name="instrumentService"></param>
    /// <param name="orderBookService"></param>
    /// <param name="priceInfoService"></param>
    /// <param name="whiteListService"></param>
    /// <param name="corporateActionService"></param>
    /// <param name="tradingSignService"></param>
    public KafkaMessageHandlerDependencies
    (
        IMongoService<InstrumentDetail> instrumentDetailService,
        IMongoService<Instrument> instrumentService,
        IMongoService<OrderBook> orderBookService,
        IMongoService<PriceInfo> priceInfoService,
        IMongoService<WhiteList> whiteListService,
        IMongoService<CorporateAction> corporateActionService,
        IMongoService<TradingSign> tradingSignService
    )
    {
        InstrumentDetailService =
            instrumentDetailService ?? throw new ArgumentNullException(nameof(instrumentDetailService));
        InstrumentService = instrumentService ?? throw new ArgumentNullException(nameof(instrumentService));
        OrderBookService = orderBookService ?? throw new ArgumentNullException(nameof(orderBookService));
        PriceInfoService = priceInfoService ?? throw new ArgumentNullException(nameof(priceInfoService));
        WhiteListService = whiteListService ?? throw new ArgumentNullException(nameof(whiteListService));
        CorporateActionService = corporateActionService ?? throw new ArgumentNullException(nameof(corporateActionService));
        TradingSignService = tradingSignService ?? throw new ArgumentNullException(nameof(tradingSignService));
    }

    public IMongoService<InstrumentDetail> InstrumentDetailService { get; set; }
    public IMongoService<Instrument> InstrumentService { get; set; }
    public IMongoService<OrderBook> OrderBookService { get; set; }
    public IMongoService<PriceInfo> PriceInfoService { get; set; }
    public IMongoService<WhiteList> WhiteListService { get; set; }
    public IMongoService<CorporateAction> CorporateActionService { get; set; }
    public IMongoService<TradingSign> TradingSignService { get; set; }
}