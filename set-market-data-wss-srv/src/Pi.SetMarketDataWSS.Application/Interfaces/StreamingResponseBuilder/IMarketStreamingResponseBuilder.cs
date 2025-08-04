using Pi.SetMarketDataWSS.Domain.Models.Response;

namespace Pi.SetMarketDataWSS.Application.Interfaces.StreamingResponseBuilder;

public interface IMarketStreamingResponseBuilder
{
    IMarketStreamingResponseBuilder WithOrderBookId(string orderBookId, string messageType);

    Task<IMarketStreamingResponseBuilder> FetchDataAsync(FetchDataParams fetchDataParams);

    MarketStreamingResponse Build(int decimalsInPrice);
}

public class FetchDataParams
{
    public string? OriginalPriceInfoCached { get; set; }
    public string? PriceInfoCached { get; set; }
    public string? MarketByPriceCached { get; set; }
    public string? PublicTradeCached { get; set; }
    public string? OrderBookStateCached { get; set; }
    public string? InstrumentDetailCached { get; set; }
    public string? MarketDirectoryCached { get; set; }
    public string? OpenInterestCached { get; set; }
    public string? StreamingBody { get; set; }
    public string? UnderlyingStreamingBody { get; set; }
}