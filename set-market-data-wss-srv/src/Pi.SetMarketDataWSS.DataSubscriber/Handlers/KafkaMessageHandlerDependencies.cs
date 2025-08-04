using Pi.SetMarketDataWSS.Application.Interfaces.ItchHousekeeper;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Interfaces.StreamingResponseBuilder;
using Pi.SetMarketDataWSS.DataSubscriber.BidOfferMapper;
using Pi.SetMarketDataWSS.DataSubscriber.Services;

namespace Pi.SetMarketDataWSS.DataSubscriber.Handlers;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
public class KafkaMessageHandlerDependencies
{
    /// <summary>
    /// </summary>
    /// <param name="marketStreamingResponseBuilder"></param>
    /// <param name="itchMapperService"></param>
    /// <param name="itchHousekeeperService"></param>
    /// <param name="bidOfferService"></param>
    /// <param name="taskQueue"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public KafkaMessageHandlerDependencies(IMarketStreamingResponseBuilder marketStreamingResponseBuilder,
        IItchMapperService itchMapperService,
        IItchHousekeeperService itchHousekeeperService,
        IBidOfferService bidOfferService,
        BackgroundTaskQueue taskQueue)
    {
        MarketStreamingResponseBuilder =
            marketStreamingResponseBuilder ?? throw new ArgumentNullException(nameof(marketStreamingResponseBuilder));
        ItchMapperService = itchMapperService ?? throw new ArgumentNullException(nameof(itchMapperService));
        ItchHousekeeperService =
            itchHousekeeperService ?? throw new ArgumentNullException(nameof(itchHousekeeperService));
        BidOfferService = bidOfferService ?? throw new ArgumentNullException(nameof(bidOfferService));
        TaskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
    }

    public IMarketStreamingResponseBuilder MarketStreamingResponseBuilder { get; set; }
    public IItchMapperService ItchMapperService { get; set; }
    public IItchHousekeeperService ItchHousekeeperService { get; set; }
    public IBidOfferService BidOfferService { get; set; }
    public BackgroundTaskQueue TaskQueue { get; set; }
}