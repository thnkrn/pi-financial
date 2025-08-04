using Pi.SetMarketDataWSS.Domain.Entities;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using PublicTrade = Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper.PublicTrade;

namespace Pi.SetMarketDataWSS.Application.Services.Models.StreamingResponseBuilder;

public class BuilderContext
{
    public string? OrderBookId { get; init; }
    public string? MessageType { get; init; }
    public PriceInfo? OriginalPriceInfo { get; set; }
    public PriceInfo? PriceInfo { get; set; }
    public OrderBook? MarketByPrice { get; set; }
    public PublicTrade[]? PublicTrades { get; set; }
    public MarketStatus? OrderBookState { get; set; }
    public InstrumentDetail? InstrumentDetail { get; set; }
    public MarketDirectory? MarketDirectory { get; set; }
    public OpenInterest? OpenInterest { get; set; }
    public MarketStreamingResponse? StreamingBody { get; set; }
    public MarketStreamingResponse? UnderlyingStreamingBody { get; set; }
    public bool DataFetched { get; set; }
}